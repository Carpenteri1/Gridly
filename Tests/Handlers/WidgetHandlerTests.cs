using Gridly.Handlers;
using Gridly.Models;
using Gridly.Querys;
using Gridly.Tests.Infrastructure;

namespace Gridly.Tests.Handlers;

public class WidgetHandlerTests
{
    [Fact]
    public async Task Handle_WhenWidgetsExist_ReturnsOkWithList()
    {
        var repository = new FakeWidgetRepository
        {
            Widgets =
            [
                new WidgetModel { Id = 1, WidgetType = "Empty", Label = "Empty", Description = "", Icon = "" },
                new WidgetModel { Id = 2, WidgetType = "Custom", Label = "Custom", Description = "", Icon = "" },
            ],
        };
        var handler = new WidgetHandler(repository);

        var result = await handler.Handle(new GetWidgetQuery(), CancellationToken.None);

        var widgets = ResultAssertions.AssertOk<List<WidgetModel>>(result);
        Assert.Equal(2, widgets.Count);
    }

    [Fact]
    public async Task Handle_WhenNoWidgetsExist_ReturnsNoContent()
    {
        var repository = new FakeWidgetRepository();
        var handler = new WidgetHandler(repository);

        var result = await handler.Handle(new GetWidgetQuery(), CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status204NoContent);
    }
}
