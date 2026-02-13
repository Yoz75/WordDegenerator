
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GUI;
public sealed class GraphCreatorTemplateSelector : DataTemplateSelector
{
    public DataTemplate BlockCreatorTemplate { get; set; }
    public DataTemplate SeparatorCreatorTemplate { get; set; }
    public DataTemplate RandomBlockTokenizerTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if(item is null)
        {
            return BlockCreatorTemplate;
        }
        else if(item is StageWrapper wrapper)
        {
            var type = wrapper.SelectedTokenizerType;
            return type switch
            {
                TokenizerType.BlockTokenizer => BlockCreatorTemplate,
                TokenizerType.SeparatorTokenizer => SeparatorCreatorTemplate,
                TokenizerType.RandomBlockTokenizer => RandomBlockTokenizerTemplate,
                _ => throw new System.ArgumentException("Unknown graph creator template!")
            };
        }

        throw new System.ArgumentException($"Item is not a GraphCreatorType! It is {item.GetType().Name}");
    }
}
