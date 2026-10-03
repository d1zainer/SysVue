using Avalonia.Controls;
using Avalonia.Platform;
using SystemProgramm.Models;
using SystemProgramm.Services;

namespace SystemProgramm.Views;

public sealed class Tray : IDisposable
{
    private static readonly SectionKind[] Lines = [SectionKind.Cpu, SectionKind.Memory, SectionKind.Gpu];

    private readonly HardwareSampler _sampler;

    private readonly TrayIcon _icon;

    public Tray(HardwareSampler sampler)
    {
        _sampler = sampler;

        var open = new NativeMenuItem(Localization.TrayOpen);
        var exit = new NativeMenuItem(Localization.TrayExit);

        open.Click += (_, _) => Opened?.Invoke(this, EventArgs.Empty);
        exit.Click += (_, _) => ExitRequested?.Invoke(this, EventArgs.Empty);

        var menu = new NativeMenu();

        menu.Items.Add(open);
        menu.Items.Add(exit);

        _icon = new TrayIcon
        {
            Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://SysVue/Assets/soft_icon.ico"))),
            ToolTipText = $"{Localization.AppTitle} {Services.Build.Version}",
            Menu = menu,
            IsVisible = true
        };

        _icon.Clicked += (_, _) => Opened?.Invoke(this, EventArgs.Empty);

        if (sampler.Latest is { } readings)
        {
            Describe(readings);
        }

        sampler.Updated += OnUpdated;
    }

    public event EventHandler? Opened;

    public event EventHandler? ExitRequested;

    public void Dispose()
    {
        _sampler.Updated -= OnUpdated;
        _icon.Dispose();
    }

    private void OnUpdated(object? sender, IReadOnlyList<HardwareReading> readings)
    {
        Describe(readings);
    }
    
    private void Describe(IReadOnlyList<HardwareReading> readings)
    {
        var cards = _sampler.Readers
            .Zip(readings, (reader, reading) => (reader.Section, reader.Title, reading.LoadText))
            .ToArray();

        var text = new List<string> { $"{Localization.AppTitle} {Services.Build.Version}" };

        foreach (var section in Lines)
        {
            var card = cards.FirstOrDefault(item => item.Section == section);

            if (card.LoadText is { } load)
            {
                text.Add($"{card.Title}  {load}");
            }
        }

        _icon.ToolTipText = string.Join(Environment.NewLine, text);
    }
}