using Gridly.helpers;
using Gridly.Models;
using Gridly.Tests.Infrastructure;

namespace Gridly.Tests.Helpers;

public class ComponentHandlerHelperTests
{
    [Fact]
    public void IconDataHasValue_ReturnsTrue_WhenAllFieldsExist()
    {
        var helper = new ComponentHandlerHelper(new FakeFileService());
        var icon = new IconModel { Name = "grid", Type = "svg", Base64Data = "Zm9v" };

        var result = helper.IconDataHasValue(icon);

        Assert.True(result);
    }

    [Fact]
    public void IconDataHasValue_ReturnsFalse_WhenIconIsNull()
    {
        var helper = new ComponentHandlerHelper(new FakeFileService());

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
        var helper = new ComponentHandlerHelper(new FakeFileService());
        var icon = new IconModel
        {
            Name = name!,
            Type = type!,
            Base64Data = base64Data!
        };

        var result = helper.IconDataHasValue(icon);

        Assert.False(result);
    }

    [Fact]
    public void UploadIcon_DelegatesToFileService()
    {
        var fileService = new FakeFileService { UploadIconResult = true };
        var helper = new ComponentHandlerHelper(fileService);
        var component = new ComponentModel { IconData = new IconModel { Name = "grid", Type = "svg", Base64Data = "Zm9v" } };

        var result = helper.UploadIcon(component);

        Assert.True(result);
        Assert.Same(component.IconData, fileService.UploadedIcon);
    }

    [Fact]
    public void DeleteIcon_DelegatesToFileService()
    {
        var fileService = new FakeFileService { DeleteIconResult = true };
        var helper = new ComponentHandlerHelper(fileService);
        var component = new ComponentModel { IconData = new IconModel { Name = "grid", Type = "svg" } };

        var result = helper.DeleteIcon(component);

        Assert.True(result);
        Assert.Equal(("grid", "svg"), fileService.DeletedIcon);
    }

    [Fact]
    public void SetIndexValues_ReassignsSequentialOneBasedIndexes()
    {
        var helper = new ComponentHandlerHelper(new FakeFileService());
        var components = new List<ComponentModel>
        {
            new() { Id = 11, IndexPosition = 99 },
            new() { Id = 17, IndexPosition = 42 },
            new() { Id = 23, IndexPosition = null }
        };

        var result = helper.SetIndexValues(components).ToArray();

        Assert.Equal(new int?[] { 1, 2, 3 }, result.Select(x => x.IndexPosition).ToArray());
        Assert.Equal(new[] { 11, 17, 23 }, result.Select(x => x.Id).ToArray());
    }
}
