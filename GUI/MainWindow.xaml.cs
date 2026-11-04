using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        public WDViewModel WDVM { get; }
        private TitleBarRegionsController RegionsController;

        public MainWindow()
        {
            InitializeComponent();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(TitleBar);

            TitleBarRegionsController.Settings settings = new()
            {
                TitleBar = TitleBar,
                LeftPaddingColumn = LeftPaddingColumn,
                RightPaddingColumn = RightPaddingColumn,
                AppWindow = AppWindow
            };

            TitleBar.Loaded += OnTitleBarLoaded;
            TitleBar.SizeChanged += OnTitlebarSizeChanged;
            
            RegionsController = new(settings);
                        
            WDVM = new(WinRT.Interop.WindowNative.GetWindowHandle(this));
        }

        private void OnAddStageButtonClick(object sender, RoutedEventArgs e)
        {
            WDVM.AddStage();
        }

        private void SetPassthroughElements()
        {
            RegionsController.SetPassthroughElements(TitleBarIcon, TitleBarTextBlock, FileFlyoutButton, ExecuteButton);
        }

        private void OnTitleBarLoaded(object sender, RoutedEventArgs e)
        {
            SetPassthroughElements();
        }

        private void OnTitlebarSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetPassthroughElements();
        }

        private void RemoveStageButtonClick(object sender, RoutedEventArgs e)
        {
            if(sender is Button { DataContext: StageWrapper wrapper})
            {
                WDVM.RemoveStage(wrapper);
            }
        }
    }
}
