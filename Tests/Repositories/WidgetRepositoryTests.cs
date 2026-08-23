using Gridly.Data;
using Gridly.Entities;
using Gridly.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Tests.Repositories;

public sealed class WidgetRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly GridlyDbContext _dbContext;
    private readonly WidgetRepository _repository;

    public WidgetRepositoryTests()
    {
        _connection.Open();
        _dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(_connection).Options);
        _dbContext.Database.EnsureCreated();
        _repository = new WidgetRepository(_dbContext);
    }

    [Fact]
    public async Task Get_ReturnsAllWidgetsWithJoinedWidgetTypeName()
    {
        var widgetType = new WidgetTypeEntity { Name = "Weather" };
        _dbContext.WidgetTypes.Add(widgetType);
        await _dbContext.SaveChangesAsync();
        _dbContext.Widgets.Add(new WidgetEntity
        {
            WidgetType = widgetType.Id, Label = "Weather widget", Description = "", Icon = "clouds",
        });
        await _dbContext.SaveChangesAsync();

        var result = (await _repository.Get()).ToList();

        var widget = Assert.Single(result);
        Assert.Equal("Weather", widget.WidgetType);
        Assert.Equal("Weather widget", widget.Label);
        Assert.Equal("clouds", widget.Icon);
    }

    [Fact]
    public async Task Get_WhenWidgetTypeIsNull_ReturnsEmptyWidgetTypeString()
    {
        _dbContext.Widgets.Add(new WidgetEntity
        {
            WidgetType = null, Label = "Custom widget", Description = "", Icon = "box_add",
        });
        await _dbContext.SaveChangesAsync();

        var result = (await _repository.Get()).ToList();

        var widget = Assert.Single(result);
        Assert.Equal(string.Empty, widget.WidgetType);
    }

    [Fact]
    public async Task Get_WhenNoWidgetsExist_ReturnsEmpty()
    {
        var result = await _repository.Get();

        Assert.Empty(result);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
        SqliteConnection.ClearAllPools();
    }
}
