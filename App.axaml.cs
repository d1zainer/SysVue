using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using SystemProgramm.Services;
using SystemProgramm.ViewModels;
using SystemProgramm.Views;

namespace SystemProgramm;

public partial class App : Application
{
    private readonly AppStore _store = new();

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            CultureSetup.Apply(_store.Language);
            ApplyTheme();
            
            if (_store.ElevateOnStart())
            {
                Environment.Exit(0);
                return;
            }

            _store.Open();
            desktop.Exit += (_, _) => _store.Dispose();
            _store.ExitRequested += (_, _) => desktop.Shutdown();

            var tray = new Tray(_store.Overview);

            tray.Opened += (_, _) =>
            {
                if (desktop.MainWindow is not { } window)
                {
                    return;
                }

                window.WindowState = WindowState.Normal;
                window.Show();
                window.Activate();
            };

            tray.ExitRequested += (_, _) => desktop.Shutdown();
            desktop.Exit += (_, _) => tray.Dispose();

            _store.ThemeChanged += (_, _) => ApplyTheme();
            _store.LanguageChanged += (_, _) =>
            {
                CultureSetup.Apply(_store.Language);
                Recreate(desktop);
            };

            desktop.MainWindow = CreateMainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ApplyTheme() =>
        RequestedThemeVariant = _store.Theme switch
        {
            "Light" => ThemeVariant.Light,
            "Dark" => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };

    private MainWindow CreateMainWindow() =>
        new() { DataContext = new MainWindowViewModel(_store) };

    private void Recreate(IClassicDesktopStyleApplicationLifetime desktop)
    {
        var previous = desktop.MainWindow;
        var replacement = CreateMainWindow();

        desktop.MainWindow = replacement;
        replacement.Show();
        previous?.Close();

        (previous?.DataContext as IDisposable)?.Dispose();
    }
}
