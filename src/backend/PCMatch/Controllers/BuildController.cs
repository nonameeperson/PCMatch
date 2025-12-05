using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCMatch.Data;
using PCMatch.Models;
using PCMatch.Services;
using System.Text;

namespace PCMatch.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuildController : ControllerBase
{
    private readonly PcMatchDbContext _context;

    public BuildController(PcMatchDbContext context)
    {
        _context = context;
    }

    [HttpGet("components")]
    public async Task<IActionResult> GetComponents(
        [FromQuery] string? type,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? search)
    {
        var query = _context.Components.AsQueryable();

        if (!string.IsNullOrEmpty(type)) query = query.Where(c => c.ComponentType == type);
        if (minPrice.HasValue) query = query.Where(c => c.Price >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(c => c.Price <= maxPrice.Value);
        if (!string.IsNullOrEmpty(search)) query = query.Where(c => c.Name.Contains(search));

        var components = await query.ToListAsync();
        var result = new List<object>();

        foreach (var c in components)
        {
            string desc = "";

            switch (c.ComponentType)
            {
                case "Cpu":
                    var cpu = _context.Cpus.FirstOrDefault(x => x.Id == c.Id);
                    if (cpu != null) desc = $"{cpu.Cores} ядер / {cpu.Threads} потоків, {cpu.Socket}, {cpu.BaseClockGhz}-{cpu.BoostClockGhz} GHz";
                    break;
                case "Gpu":
                    var gpu = _context.Gpus.FirstOrDefault(x => x.Id == c.Id);
                    if (gpu != null) desc = $"{gpu.VramGB} GB пам'яті, Score: {gpu.PerformanceScore}";
                    break;
                case "Motherboard":
                    var mb = _context.Motherboards.FirstOrDefault(x => x.Id == c.Id);
                    if (mb != null) desc = $"{mb.Socket}, {mb.Chipset}, {mb.RamType}, {mb.FormFactor}";
                    break;
                case "Ram":
                    var ram = _context.Rams.FirstOrDefault(x => x.Id == c.Id);
                    if (ram != null) desc = $"{ram.CapacityGB} GB, {ram.ModuleCount} планки, {ram.SpeedMhz} MHz, {ram.RamType}";
                    break;
                case "Psu":
                    var psu = _context.Psus.FirstOrDefault(x => x.Id == c.Id);
                    if (psu != null) desc = $"{psu.Wattage} W, {psu.Rating}";
                    break;
                case "Storage":
                    var st = _context.Storages.FirstOrDefault(x => x.Id == c.Id);
                    if (st != null) desc = $"{st.CapacityGB} GB, {st.Type}";
                    break;
                case "Case":
                    var ca = _context.Cases.FirstOrDefault(x => x.Id == c.Id);
                    if (ca != null) desc = $"{ca.FormFactor}";
                    break;
            }

            result.Add(new
            {
                c.Id,
                c.Name,
                c.Price,
                c.ComponentType,
                c.Manufacturer,
                Description = desc
            });
        }

        return Ok(result);
    }
    [HttpGet("forecast")]
    public async Task<IActionResult> GetFpsForecast(Guid cpuId, Guid gpuId)
    {
        var cpu = await _context.Cpus.Include(c => c.Component).FirstOrDefaultAsync(c => c.Id == cpuId);
        var gpu = await _context.Gpus.Include(g => g.Component).FirstOrDefaultAsync(g => g.Id == gpuId);

        if (cpu == null || gpu == null)
            return BadRequest("Component not found");

        var benchmarks = await _context.FpsBenchmarks
            .Include(b => b.Game)
            .Where(b => b.CpuId == cpuId && b.GpuId == gpuId)
            .Select(b => new
            {
                Game = b.Game.Name,
                Fps = b.AverageFps,
                Source = "Database Benchmark"
            })
            .ToListAsync();

        if (!benchmarks.Any() || benchmarks.Any(b => b.Fps > 1000))
        {
            benchmarks.Clear();
            var games = await _context.Games.ToListAsync();
            foreach (var game in games)
            {
                benchmarks.Add(new
                {
                    Game = game.Name,
                    Fps = FpsCalculator.Calculate(game.Name, cpu.PerformanceScore, gpu.PerformanceScore),
                    Source = "Calculated Live"
                });
            }
        }

        return Ok(new
        {
            Build = $"{cpu.Component.Name} + {gpu.Component.Name}",
            Results = benchmarks
        });
    }

    [HttpGet("validate")]
    public async Task<IActionResult> ValidateBuild(
        Guid cpuId,
        Guid motherboardId,
        Guid? gpuId,
        Guid? psuId)
    {
        var errors = new List<string>();

        var cpu = await _context.Cpus.Include(c => c.Component).FirstOrDefaultAsync(c => c.Id == cpuId);
        var motherboard = await _context.Motherboards.Include(m => m.Component).FirstOrDefaultAsync(m => m.Id == motherboardId);

        var gpu = gpuId.HasValue
            ? await _context.Gpus.Include(g => g.Component).FirstOrDefaultAsync(g => g.Id == gpuId)
            : null;

        var psu = psuId.HasValue
            ? await _context.Psus.Include(p => p.Component).FirstOrDefaultAsync(p => p.Id == psuId)
            : null;

        if (cpu == null || motherboard == null)
        {
            return BadRequest("Необхідно вибрати Процесор та Материнську плату.");
        }

        if (cpu.Socket != motherboard.Socket)
        {
            errors.Add($"Несумісність: Процесор вимагає сокет {cpu.Socket}, а материнська плата має {motherboard.Socket}.");
        }

        if (psu != null)
        {
            int cpuTdp = cpu.Component.Tdp ?? 65;
            int gpuTdp = gpu?.Component.Tdp ?? 0;

            int estimatedLoad = cpuTdp + gpuTdp + 100;

            if (psu.Wattage < estimatedLoad)
            {
                errors.Add($"Нестача живлення: Ваша система споживає приблизно {estimatedLoad}W, а блок живлення має лише {psu.Wattage}W.");
            }
        }

        bool isValid = !errors.Any();

        return Ok(new
        {
            IsValid = isValid,
            Messages = isValid ? new List<string> { "Збірка сумісна!" } : errors,
            EstimatedWattage = psu != null ? (cpu.Component.Tdp ?? 65) + (gpu?.Component.Tdp ?? 0) + 100 : 0
        });
    }

    [HttpGet("purposes")]
    public async Task<IActionResult> GetPurposes()
    {
        return Ok(await _context.Purposes.ToListAsync());
    }

    [HttpGet("recommend")]
    public async Task<IActionResult> RecommendBuild([FromQuery] string purpose, [FromQuery] decimal budget)
    {
        decimal gpuShare = 0.35m;
        decimal cpuShare = 0.20m;

        if (purpose.Contains("Office", StringComparison.OrdinalIgnoreCase))
        {
            gpuShare = 0.0m;
            cpuShare = 0.30m;
        }

        var allCpus = await _context.Cpus.Include(c => c.Component).ToListAsync();
        var allGpus = await _context.Gpus.Include(g => g.Component).ToListAsync();
        var allMobos = await _context.Motherboards.Include(m => m.Component).ToListAsync();
        var allRams = await _context.Rams.Include(r => r.Component).ToListAsync();
        var allPsus = await _context.Psus.Include(p => p.Component).ToListAsync();
        var allCases = await _context.Cases.Include(c => c.Component).ToListAsync();
        var allStorages = await _context.Storages.Include(s => s.Component).ToListAsync();

        try
        {
            var targetGpuPrice = budget * gpuShare;
            var bestGpu = allGpus
                .Where(g => g.Component.Price <= targetGpuPrice)
                .OrderByDescending(g => g.PerformanceScore)
                .FirstOrDefault();

            if (gpuShare > 0 && bestGpu == null)
                return BadRequest("Бюджет замалий для ігрової відеокарти.");

            var targetCpuPrice = budget * cpuShare;
            var bestCpu = allCpus
                .Where(c => c.Component.Price <= targetCpuPrice)
                .Where(c => gpuShare > 0 || c.HasIntegratedGraphics)
                .OrderByDescending(c => c.PerformanceScore)
                .FirstOrDefault();

            if (bestCpu == null) return BadRequest("Бюджет замалий для процесора.");

            var bestMobo = allMobos
                .Where(m => m.Socket == bestCpu.Socket)
                .Where(m => m.Component.Price <= budget * 0.15m)
                .OrderByDescending(m => m.Component.Price)
                .FirstOrDefault();

            if (bestMobo == null) return BadRequest($"Не знайдено материнську плату для сокета {bestCpu.Socket} в цьому бюджеті.");

            var bestRam = allRams
                .Where(r => r.RamType == bestMobo.RamType)
                .Where(r => r.Component.Price <= budget * 0.10m)
                .OrderByDescending(r => r.CapacityGB)
                .FirstOrDefault();

            int estimatedWattage = (bestCpu.Component.Tdp ?? 65) + (bestGpu?.Component.Tdp ?? 0) + 100;
            var bestPsu = allPsus
                .Where(p => p.Wattage >= estimatedWattage)
                .Where(p => p.Component.Price <= budget * 0.10m)
                .OrderBy(p => p.Component.Price)
                .FirstOrDefault();

            var bestStorage = allStorages.Where(s => s.Component.Price <= budget * 0.10m).OrderByDescending(s => s.CapacityGB).FirstOrDefault();
            var bestCase = allCases.Where(c => c.Component.Price <= budget * 0.10m).FirstOrDefault();

            if (bestRam == null || bestPsu == null || bestStorage == null || bestCase == null)
                return BadRequest("Не вдалося підібрати повний комплект у цей бюджет.");

            var totalPrice = bestCpu.Component.Price + bestMobo.Component.Price + bestRam.Component.Price +
                             (bestGpu?.Component.Price ?? 0) + bestPsu.Component.Price +
                             bestStorage.Component.Price + bestCase.Component.Price;

            return Ok(new
            {
                TotalPrice = totalPrice,
                RemainingBudget = budget - totalPrice,
                Build = new
                {
                    Cpu = bestCpu.Component,
                    Gpu = bestGpu?.Component,
                    Motherboard = bestMobo.Component,
                    Ram = bestRam.Component,
                    Psu = bestPsu.Component,
                    Storage = bestStorage.Component,
                    Case = bestCase.Component
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Помилка підбору: {ex.Message}");
        }
    }
}