using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WD.Core;
using WD.Core.TextGenerators;
using WD.Core.Tokenizers;
using Windows.Storage;
using Windows.Storage.Pickers;
namespace GUI;

/*
    ***************************************************************************************************************************************************************
    There are many expensive expressions (like is) in this file, but, anyway, they are placed in non-perfomance-critical sections. The longest thing is tokenization
    ***************************************************************************************************************************************************************
 */

public enum TokenizerType : byte
{
    BlockTokenizer,
    SeparatorTokenizer,
    RandomBlockTokenizer,
    SmartTokenizer
}

public enum TextGeneratorType : byte
{
    Default
}

public class StageWrapper : ObservableObject
{
    public StageWrapper Self { get => this; }

    public string Name
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
        }
    } = "New Stage"; 

    private WDGenerationStage Stage_;
    public ref WDGenerationStage Stage
    {
        get => ref Stage_;
    }

    public int RepeatTimes
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
            Stage.RepeatTimes = (uint)value;
        }
    } = 1;

    // Yeah, literally many variables for different creators like in Word generator 2
    public int BlockSize
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Tokenizer is BlockTokenizer blockTokenizer)
            {
                blockTokenizer.BlockSize = value;
            }

            ClearCash();
        }
    }

    public string Separator
    {
        get => field;
        set
        {
            if(value.Length > 1) value = value.Substring(0, 1);

            SetProperty(ref field, value);
            if(Stage.Tokenizer is SeparatorTokenizer separatorTokenizer)
            {
                separatorTokenizer.Separator = value[0];
            }

            ClearCash();
        }
    }

    public int MinBlockSize
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Tokenizer is RandomBlockTokenizer tokenizer)
            {
                tokenizer.Min = value;
            }

            ClearCash();
        }
    }

    public int MaxBlockSize
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Tokenizer is RandomBlockTokenizer tokenizer)
            {
                tokenizer.Max = value;
            }

            ClearCash();
        }
    }

    public int IterationsCount
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Tokenizer is SmartTokenizer tokenizer)
            {
                tokenizer.IterationsCount = value;
            }

            ClearCash();
        }
    }
    
    public int MaxTokenLength
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Tokenizer is SmartTokenizer tokenizer)
            {
                tokenizer.MaxTokenLength = value;
            }

            ClearCash();
        }
    }

    public int TokenizerTopK
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Tokenizer is SmartTokenizer tokenizer)
            {
                tokenizer.TopK = value;
            }

            ClearCash();
        }
    }

    // Generator parameters:
    public int OutputSize
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Generator is DefaultTextGenerator defaultGenerator)
            {
                defaultGenerator.OutputSize = value;
            }
        }
    }

    public int TopK
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Generator is DefaultTextGenerator defaultGenerator)
            {
                defaultGenerator.TopK = value;
            }
        }
    }

    public float RandomTokenChance
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Generator is DefaultTextGenerator defaultGenerator)
            {
                defaultGenerator.RandomTokenChance = value;
            }
        }
    }

    public bool UseDebugSeparator
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Generator is DefaultTextGenerator defaultGenerator)
            {
                defaultGenerator.UseSeparator = value;
            }
        }
    }

    public ObservableCollection<TokenizerType> TokenizerTypes { get; } = [];

    public ObservableCollection<TextGeneratorType> GeneratorTypes { get; } =
    [
        TextGeneratorType.Default
    ];

    public TokenizerType SelectedTokenizerType
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
            UpdateTokenizerUsageProperties();

            Stage.Tokenizer = value switch
            {
                TokenizerType.BlockTokenizer => Stage.Tokenizer is not BlockTokenizer ? new BlockTokenizer() : Stage.Tokenizer,
                TokenizerType.SeparatorTokenizer => Stage.Tokenizer is not SeparatorTokenizer ? new SeparatorTokenizer() : Stage.Tokenizer,
                TokenizerType.RandomBlockTokenizer => Stage.Tokenizer is not RandomBlockTokenizer ? new RandomBlockTokenizer() : Stage.Tokenizer,
                TokenizerType.SmartTokenizer => Stage.Tokenizer is not SmartTokenizer ? new SmartTokenizer() : Stage.Tokenizer,
                _ => throw new ArgumentException("Selected creator type can't be added to stage!"),
            };
        }
    } = TokenizerType.BlockTokenizer;

    private void UpdateTokenizerUsageProperties()
    {
        OnPropertyChanged(nameof(UsingBlockTokenizer));
        OnPropertyChanged(nameof(UsingSeparatorTokenizer));
        OnPropertyChanged(nameof(UsingRandomTokenizer));
        OnPropertyChanged(nameof(UsingSmartTokenizer));
    }

    public bool UsingBlockTokenizer => Stage.Tokenizer is BlockTokenizer;
    public bool UsingSeparatorTokenizer => Stage.Tokenizer is SeparatorTokenizer;
    public bool UsingRandomTokenizer => Stage.Tokenizer is RandomBlockTokenizer;
    public bool UsingSmartTokenizer => Stage.Tokenizer is SmartTokenizer;

    public bool UsingDefaultGenerator => Stage.Generator is DefaultTextGenerator;

    public TextGeneratorType SelectedGeneratorType
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
            UpdateGeneratorUsageProps();

            switch(value)
            {
                case TextGeneratorType.Default:
                    Stage.Generator = Stage.Generator is not DefaultTextGenerator ? new DefaultTextGenerator() : Stage.Generator;
                    break;

                default: throw new ArgumentException("Selected generator type can't be added to stage!");
            }
        }
    }

    private void UpdateGeneratorUsageProps()
    {
        OnPropertyChanged(nameof(UsingBlockTokenizer));
    }


    public void ClearCash()
    {
        if(Stage.Tokenizer is CashTokenizer tokenizer)
        {
            tokenizer.ClearCash();
        }
    }

    public StageWrapper()
    {
        SelectedGeneratorType = GeneratorTypes[0];
        BlockSize = 3;
        OutputSize = 50;
        TopK = 5;

        // Fuck c#, I could do this at compile time in D
        TokenizerType[] values = (TokenizerType[])Enum.GetValues<TokenizerType>();

        foreach( var value in values)
        {
            TokenizerTypes.Add(value);
        }

        SelectedTokenizerType = TokenizerTypes[0];
    }
}

public class WDViewModel : ObservableObject
{
    private nint WindowHandle;

    public WDViewModel(nint windowHandle)
    {
        WindowHandle = windowHandle;
    }

    public string InputText { get => field; set => SetProperty(ref field, value); } = "Enter the text here...";
    public string ResultText
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
        }
    } = "Chevosekrofelyab";

    public ObservableCollection<StageWrapper> Stages { get; } = new();
    private Task<string>? ExecuteTask;

    public void AddStage(object sender, RoutedEventArgs e)
    {
        Stages.Add(new());
    }

    public async Task ExecuteAsync()
    {
        if(ExecuteTask is not null && !ExecuteTask.IsCompleted) return;

        ResultText = "Executing word degeneration...";

        var input = InputText;
        var stagesSnapshot = Stages.Select(s => s.Stage).ToArray();

        ExecuteTask = Task.Run(() =>
        {
            var degenerator = new WordDegenerator();
            foreach(var snapshot in stagesSnapshot)
                degenerator.Stages.Add(snapshot);

            return degenerator.Execute(input);
        });

        ResultText = await ExecuteTask;
    }

    public async void LoadFileAsync()
    {
        FileOpenPicker picker = new();
        picker.ViewMode = PickerViewMode.List;
        picker.FileTypeFilter.Add(".txt");

        WinRT.Interop.InitializeWithWindow.Initialize(picker, WindowHandle);

        StorageFile file = await picker.PickSingleFileAsync();
        InputText = await File.ReadAllTextAsync(file.Path); //bruh that's a lot simpler than stream shit
    }
}