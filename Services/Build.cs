using System.Reflection;

namespace SystemProgramm.Services;

public static class Build
{
    public static string Version { get; } = Describe();

    private static string Describe()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;

        return version is null ? "?" : $"{version.Major}.{version.Minor}.{version.Build}";
    }
}