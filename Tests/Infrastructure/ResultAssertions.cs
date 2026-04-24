using Microsoft.AspNetCore.Http;

namespace Gridly.Tests.Infrastructure;

internal static class ResultAssertions
{
    public static T AssertOk<T>(IResult result)
    {
        var statusResult = Assert.IsAssignableFrom<IStatusCodeHttpResult>(result);
        Assert.Equal(StatusCodes.Status200OK, statusResult.StatusCode);

        var valueResult = Assert.IsAssignableFrom<IValueHttpResult>(result);
        return Assert.IsType<T>(valueResult.Value);
    }

    public static void AssertStatusCode(IResult result, int expectedStatusCode)
    {
        var statusResult = Assert.IsAssignableFrom<IStatusCodeHttpResult>(result);
        Assert.Equal(expectedStatusCode, statusResult.StatusCode);
    }
}
