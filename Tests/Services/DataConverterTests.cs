using System.Text.Json;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.Tests.Services;

public class DataConverterTests
{
    private readonly DataConverter<VersionModel> _converter = new();

    [Fact]
    public void DeserializeJson_ReturnsObject()
    {
        const string json = """{"name":"1.2.3","newRelease":true}""";

        var result = _converter.DeserializeJson(json);

        Assert.NotNull(result);
        Assert.Equal("1.2.3", result.Name);
        Assert.True(result.NewRelease);
    }

    [Fact]
    public void DeserializeJsonToArray_ReturnsArray()
    {
        const string json = """[{"name":"1.0.0","newRelease":false},{"name":"1.1.0","newRelease":true}]""";

        var result = _converter.DeserializeJsonToArray(json);

        Assert.NotNull(result);
        Assert.Equal(2, result.Length);
        Assert.Equal("1.1.0", result[1].Name);
    }

    [Fact]
    public void DeserializeJson_InvalidPayload_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() => _converter.DeserializeJson("{"));
    }

    [Fact]
    public void SerializerToJsonString_ForObject_SerializesProperties()
    {
        var model = new VersionModel { Name = "2.0.0", NewRelease = true };

        var result = _converter.SerializerToJsonString(model);

        Assert.Equal("""{"name":"2.0.0","newRelease":true}""", result);
    }

    [Fact]
    public void SerializerToJsonString_ForEnumerable_SerializesCollection()
    {
        var models = new[]
        {
            new VersionModel { Name = "1.0.0", NewRelease = false },
            new VersionModel { Name = "2.0.0", NewRelease = true }
        };

        var result = _converter.SerializerToJsonString(models);

        Assert.Equal("""[{"name":"1.0.0","newRelease":false},{"name":"2.0.0","newRelease":true}]""", result);
    }

    [Fact]
    public void ToInt_StripsNonDigitCharacters()
    {
        var result = _converter.ToInt("v1.2.3");

        Assert.Equal(123, result);
    }

    [Fact]
    public void ToInt_WithoutDigits_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => _converter.ToInt("version"));
    }
}
