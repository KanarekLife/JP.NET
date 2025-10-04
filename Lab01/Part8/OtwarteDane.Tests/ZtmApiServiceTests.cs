using OtwarteDane.Cli.Services;
using OtwarteDane.Cli.Models;

namespace OtwarteDane.Tests;

public class ZtmApiServiceTests
{
    [Fact]
    public async Task GetDeparturesAsync_WithValidStopId_ShouldReturnData()
    {
        // Arrange
        var service = new ZtmApiService();
        var stopId = 1000; // Plac Solidarności

        // Act
        var result = await service.GetDeparturesAsync(stopId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.LastUpdate > DateTime.MinValue);
        service.Dispose();
    }

    [Fact]
    public async Task GetStopsAsync_ShouldReturnStopsData()
    {
        // Arrange
        var service = new ZtmApiService();

        // Act
        var result = await service.GetStopsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        service.Dispose();
    }

    [Fact]
    public async Task GetDeparturesAsync_WithInvalidStopId_ShouldReturnNull()
    {
        // Arrange
        var service = new ZtmApiService();
        var invalidStopId = 99999999; // Nieprawdopodobny ID

        // Act
        var result = await service.GetDeparturesAsync(invalidStopId);

        // Assert - API może zwrócić puste dane zamiast błędu, więc sprawdzimy czy jest null lub puste
        if (result != null)
        {
            Assert.True(result.Departures.Count == 0);
        }
        service.Dispose();
    }
}
