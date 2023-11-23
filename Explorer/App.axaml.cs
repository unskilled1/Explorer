using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Explorer.Interfaces.MessageBus;
using Explorer.Misc.MessageBus;
using Explorer.ViewModels;
using Explorer.Views;

namespace Explorer
{
    public partial class App : Application
    {
        private readonly IMessageBus _messageBus;

        public App()
        {
            _messageBus = new MessageBus();
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainVM(_messageBus),
                };
                desktop.Exit += Desktop_Exit;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void Desktop_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            var desktop = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
            desktop.Exit -= Desktop_Exit;
            
            var mainWindowVm = (MainVM)desktop.MainWindow!.DataContext!;
            mainWindowVm.Dispose();

            _messageBus.Dispose();
        }
    }
}