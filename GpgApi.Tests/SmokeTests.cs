using Xunit;

namespace GpgApi.Tests;

public class SmokeTests
{
    [Fact]
    public void True_Is_True()
    {
        // This test ensures that:
        // * dotnet test runs
        // * xUnit is wired correctly
        // * CI fails if tests fail
        Assert.True(true);
    }
}

