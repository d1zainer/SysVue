using Avalonia.Controls;

namespace SystemProgramm.Views;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();
    
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!e.IsProgrammatic)
        {
            e.Cancel = true;
            Hide();
        }

        base.OnClosing(e);
    }
}