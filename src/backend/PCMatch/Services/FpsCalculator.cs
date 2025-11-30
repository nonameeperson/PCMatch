namespace PCMatch.Services;

public static class FpsCalculator
{
    public static int Calculate(string gameName, int cpuScore, int gpuScore)
    {
        double cpuFactor = cpuScore / 50000.0;
        double gpuFactor = gpuScore / 50000.0;

        return gameName switch
        {
            "Cyberpunk 2077" => (int)(45 + (45 * cpuFactor * 0.25) + (45 * gpuFactor * 0.75)),
            "RUST" => (int)(70 + (70 * cpuFactor * 0.60) + (70 * gpuFactor * 0.40)),
            "CS2" => (int)(180 + (180 * cpuFactor * 0.70) + (180 * gpuFactor * 0.30)),
            _ => 60
        };
    }
}