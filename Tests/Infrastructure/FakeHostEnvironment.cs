using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Gridly.Tests.Infrastructure;

internal sealed class FakeHostEnvironment : IHostEnvironment
{
    public string ApplicationName { get; set; } = "Gridly.Tests";
    public IFileProvider ContentRootFileProvider { get; set; } = null!;
    public string ContentRootPath { get; set; } = string.Empty;
    public string EnvironmentName { get; set; } = "Development";
}
