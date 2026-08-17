using Gridly.helpers;
using Gridly.Models;
using Gridly.Tests.Infrastructure;

namespace Gridly.Tests.Helpers;

public class CardHandlerHelperTests
{
    [Fact]
    public void IconDataHasValue_ReturnsTrue_WhenAllFieldsExist()
    {
        var helper = new CardHandlerHelper(new FakeFileService());
        var icon = new IconModel { Name = "grid", Type = "svg", Base64Data = "Zm9v", MaterialIcon = "dashboard" };

        var result = helper.IconDataHasValue(icon);

        Assert.True(result);
    }

    [Fact]
    public void IconDataHasValue_ReturnsFalse_WhenIconIsNull()
    {
        var helper = new CardHandlerHelper(new FakeFileService());

        var result = helper.IconDataHasValue(null!);

        Assert.False(result);
    }

    [Theory]
    [InlineData(null, "svg", "Zm9v")]
    [InlineData("grid", null, "Zm9v")]
    [InlineData("grid", "svg", null)]
    [InlineData("", "svg", "Zm9v")]
    public void IconDataHasValue_ReturnsFalse_WhenAnyRequiredFieldIsMissing(
        string? name,
        string? type,
        string? base64Data)
    {
        var helper = new CardHandlerHelper(new FakeFileService());
        var icon = new IconModel
        {
            Name = name!,
            Type = type!,
            Base64Data = base64Data!,
            MaterialIcon = "dashboard"
        };

        var result = helper.IconDataHasValue(icon);

        Assert.False(result);
    }
    
    [Fact]
    public void DeleteIcon_DelegatesToFileService()
    {
        var fileService = new FakeFileService { DeleteIconResult = true };
        var helper = new CardHandlerHelper(fileService);
        var iconData = new IconModel { Name = "grid", Type = "svg", Base64Data = "", MaterialIcon = "" };

        var result = helper.DeleteIcon(iconData);

        Assert.True(result);
        Assert.Equal(("grid", "svg"), fileService.DeletedIcon);
    }
}
