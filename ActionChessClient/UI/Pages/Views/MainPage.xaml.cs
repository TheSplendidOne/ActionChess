using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using SharedServiceLibrary;
using Newtonsoft.Json;

namespace ActionChessClient
{
    internal partial class MainPage : Page
    {
        public MainPage(EMainPageControl mainPageControl)
        {
            InitializeComponent();
            ((CMainPageViewModel) DataContext).ActualControl = mainPageControl switch
            {
                EMainPageControl.LogIn => (UIElement)new LogInControl((CMainPageViewModel) DataContext),
                EMainPageControl.SignUp => (UIElement)new SignUpControl((CMainPageViewModel) DataContext),
                EMainPageControl.Menu => (UIElement)new MenuControl((CMainPageViewModel) DataContext),
                _ => throw new InvalidEnumArgumentException($@"Unknown main page control: {mainPageControl}")
            };
        }
    }
}
