using Avalonia.Platform;
using SystemProgramm.Models;
using SystemProgramm.Services;

namespace SystemProgramm.ViewModels.Pages;

public sealed class AboutViewModel() : PageViewModel(new PageInfo(Localization.PageAbout, SectionKind.About))
{
    public string Version { get; } = string.Format(Localization.AboutVersion, Services.Build.Version);
    
    public string OnestLicense { get; } = Read("avares://SysVue/Assets/Fonts/OFL.txt");

    public string LucideLicense { get; } = Read("avares://SysVue/Assets/Licenses/Lucide.txt");

    private static string Read(string uri)
    {
        using var stream = AssetLoader.Open(new Uri(uri));
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}

