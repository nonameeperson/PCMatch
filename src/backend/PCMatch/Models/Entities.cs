using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCMatch.Models;

public class Component
{
    [Key]
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public string? Manufacturer { get; set; }
    public int? Tdp { get; set; }
    public required string ComponentType { get; set; }
}

[Table("Cpus")]
public class Cpu
{
    [Key]
    [ForeignKey("Component")]
    public Guid Id { get; set; }
    public virtual Component Component { get; set; } = null!;

    public string? Socket { get; set; }
    public int Cores { get; set; }
    public int Threads { get; set; }
    public decimal BaseClockGhz { get; set; }
    public decimal BoostClockGhz { get; set; }
    public bool HasIntegratedGraphics { get; set; }
    public int PerformanceScore { get; set; }
}

[Table("Gpus")]
public class Gpu
{
    [Key]
    [ForeignKey("Component")]
    public Guid Id { get; set; }
    public virtual Component Component { get; set; } = null!;

    public int VramGB { get; set; }
    public int RecommendedPsuWattage { get; set; }
    public int PerformanceScore { get; set; }
}

public class Game
{
    [Key]
    public Guid Id { get; set; }
    public required string Name { get; set; }
}

public class FpsBenchmark
{
    [Key]
    public Guid Id { get; set; }
    public Guid CpuId { get; set; }
    public Guid GpuId { get; set; }
    public Guid GameId { get; set; }
    public required string Resolution { get; set; }
    public required string Settings { get; set; }
    public int AverageFps { get; set; }

    [ForeignKey("GameId")]
    public virtual Game Game { get; set; } = null!;
}

[Table("Motherboards")]
public class Motherboard
{
    [Key]
    [ForeignKey("Component")]
    public Guid Id { get; set; }
    public virtual Component Component { get; set; } = null!;

    public string? Socket { get; set; }
    public string? Chipset { get; set; }
    public string? RamType { get; set; }
    public string? FormFactor { get; set; }
}

[Table("Rams")]
public class Ram
{
    [Key]
    [ForeignKey("Component")]
    public Guid Id { get; set; }
    public virtual Component Component { get; set; } = null!;

    public string? RamType { get; set; }
    public int CapacityGB { get; set; }
    public int ModuleCount { get; set; }
    public int SpeedMhz { get; set; }
}

[Table("Psus")]
public class Psu
{
    [Key]
    [ForeignKey("Component")]
    public Guid Id { get; set; }
    public virtual Component Component { get; set; } = null!;

    public int Wattage { get; set; }
    public string? Rating { get; set; }
}

[Table("Storages")]
public class Storage
{
    [Key]
    [ForeignKey("Component")]
    public Guid Id { get; set; }
    public virtual Component Component { get; set; } = null!;

    public string? Type { get; set; }
    public int CapacityGB { get; set; }
}

[Table("Cases")]
public class Case
{
    [Key]
    [ForeignKey("Component")]
    public Guid Id { get; set; }
    public virtual Component Component { get; set; } = null!;

    public string? FormFactor { get; set; }
}

[Table("Purposes")]
public class Purpose
{
    [Key]
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}