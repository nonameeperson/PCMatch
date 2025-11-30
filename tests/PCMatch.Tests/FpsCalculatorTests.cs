using Xunit;
using PCMatch.Services;

namespace PCMatch.Tests;

public class FpsCalculatorTests
{
    [Fact]
    public void Calculate_ReturnsCorrectFps_ForCyberpunk()
    {
        string game = "Cyberpunk 2077";
        int cpuScore = 30000;
        int gpuScore = 20000;

        int result = FpsCalculator.Calculate(game, cpuScore, gpuScore);

        Assert.InRange(result, 60, 70);
    }

    [Fact]
    public void Calculate_ReturnsDefault_ForUnknownGame()
    {
        int result = FpsCalculator.Calculate("Unknown Game", 50000, 50000);

        Assert.Equal(60, result);
    }
}