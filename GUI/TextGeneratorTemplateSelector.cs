
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GUI;
public sealed class TextGeneratorTemplateSelector : DataTemplateSelector
{
    public DataTemplate BasicGeneratorTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject contaner)
    {
        if(item is null) return BasicGeneratorTemplate;
        if(item is StageWrapper wrapper)
        {
            var type = wrapper.SelectedGeneratorType;
            return type switch
            {
                TextGeneratorType.Default => BasicGeneratorTemplate,
                _ => throw new System.ArgumentException("Unknown graph creator template!")
            };
        }

        throw new System.ArgumentException("Item is not a GraphCreatorType!");
    }
}
