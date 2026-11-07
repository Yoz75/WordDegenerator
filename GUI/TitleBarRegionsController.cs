
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Graphics;

namespace WD.GUI;

/// <summary>
/// A thing that can controll regions of a title bar
/// </summary>
internal sealed class TitleBarRegionsController
{
    /*
        See https://learn.microsoft.com/en-us/windows/apps/develop/title-bar
    */
    internal struct Settings
    {
        public FrameworkElement TitleBar;

        public ColumnDefinition LeftPaddingColumn, RightPaddingColumn;
        public AppWindow AppWindow;
    }

    private Settings Settings_;

    public TitleBarRegionsController(Settings settings)
    {
        Settings_ = settings;
    }

    /// <summary>
    /// Update the title bar regions
    /// </summary>
    /// <param name="titleBar"></param>
    public void UpdateTitleBarRegions()
    {
        double scale = Settings_.TitleBar.XamlRoot.RasterizationScale;

        Settings_.LeftPaddingColumn.Width = new GridLength(Settings_.AppWindow.TitleBar.RightInset / scale);
        Settings_.RightPaddingColumn.Width = new GridLength(Settings_.AppWindow.TitleBar.RightInset / scale);
    }

    /// <summary>
    /// Set elements of the titlebar that should NOT open system menu on click
    /// </summary>
    /// <param name="elements"></param>
    public void SetPassthroughElements(params FrameworkElement[] elements)
    {
        double scale = Settings_.TitleBar.XamlRoot.RasterizationScale;
        var rects = new RectInt32[elements.Length];

        int i = 0;
        foreach(var element in elements)
        {
            rects[i] = GetRectOf(element, scale);
            i++;
        }

        InputNonClientPointerSource pointerSource = InputNonClientPointerSource.GetForWindowId(Settings_.AppWindow.Id);
        pointerSource.SetRegionRects(NonClientRegionKind.Passthrough, rects);
    }

    /// <summary>
    /// Get rect of a control inside the title bar
    /// </summary>
    /// <param name="element">the element</param>
    /// <param name="scale">the scale of titlebar</param>
    /// <returns></returns>
    private static RectInt32 GetRectOf(FrameworkElement element, double scale)
    {
        var transform = element.TransformToVisual(null);
        var bounds = transform.TransformBounds(new(0, 0, element.ActualWidth, element.ActualHeight));


        return new((int)(bounds.X * scale), (int)(bounds.Y * scale), (int)(bounds.Width * scale), (int)(bounds.Height * scale));
    }
}
