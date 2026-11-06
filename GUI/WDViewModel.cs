using CommunityToolkit.Mvvm.ComponentModel;
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

/// <summary>
/// Supported tokenizers
/// </summary>
public enum TokenizerType : byte
{
    /// <summary>
    /// Uninitialzied value
    /// </summary>
    None,
    BlockTokenizer,
    SeparatorTokenizer,
    RandomBlockTokenizer,
    SmartTokenizer
}

/// <summary>
/// Supported geenrators
/// </summary>
public enum TextGeneratorType : byte
{
    /// <summary>
    /// Uninitialized value
    /// </summary>
    None,
    Default
}

public class StageWrapper : ObservableObject
{
    private WDGenerationStage Stage_;
    public ref WDGenerationStage Stage
    {
        get => ref Stage_;
    }

    public int RepeatTimes
    {
        get;
        set
        {
            SetProperty(ref field, value);
            Stage.RepeatTimes = value;
        }
    } = 1;

    // Yeah, literally many variables for different creators like in Word generator 2
    public int BlockSize
    {
        get;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Tokenizer is BlockTokenizer blockTokenizer)
            {
                blockTokenizer.BlockSize = value;
            }

            ClearCache();
        }
    } = 3;

    public string Separator
    {
        get;
        set
        {
            if(value.Length > 1) value = value.Substring(0, 1);

            SetProperty(ref field, value);
            if(Stage.Tokenizer is SeparatorTokenizer separatorTokenizer)
            {
                separatorTokenizer.Separator = value[0];
            }

            ClearCache();
        }
    } = " ";

    public int MinBlockSize
    {
        get;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Tokenizer is RandomBlockTokenizer tokenizer)
            {
                tokenizer.Min = value;
            }

            ClearCache();
        }
    } = 1;

    public int MaxBlockSize
    {
        get;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Tokenizer is RandomBlockTokenizer tokenizer)
            {
                tokenizer.Max = value;
            }

            ClearCache();
        }
    } = 3;

    public int IterationsCount
    {
        get;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Tokenizer is SmartTokenizer tokenizer)
            {
                tokenizer.IterationsCount = value;
            }

            ClearCache();
        }
    } = 1500;

    public int MaxTokenLength
    {
        get;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Tokenizer is SmartTokenizer tokenizer)
            {
                tokenizer.MaxTokenLength = value;
            }

            ClearCache();
        }
    } = 3;

    public int TokenizerTopK
    {
        get;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Tokenizer is SmartTokenizer tokenizer)
            {
                tokenizer.TopK = value;
            }

            ClearCache();
        }
    } = 3;

    public int MinPairOccurrences
    {
        get;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Tokenizer is SmartTokenizer tokenizer)
            {
                tokenizer.MinPairOccurrences = value;
            }

            ClearCache();
        }
    } = 2;

    // Generator parameters:
    public int OutputSize
    {
        get;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Generator is DefaultTextGenerator defaultGenerator)
            {
                defaultGenerator.OutputSize = value;
            }
        }
    } = 150;

    public int TopK
    {
        get;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Generator is DefaultTextGenerator defaultGenerator)
            {
                defaultGenerator.TopK = value;
            }
        }
    } = 5;

    public float RandomTokenChance
    {
        get;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Generator is DefaultTextGenerator defaultGenerator)
            {
                defaultGenerator.RandomTokenChance = value;
            }
        }
    } = 0;

    public bool UseDebugSeparator
    {
        get;
        set
        {
            SetProperty(ref field, value);
            if(Stage.Generator is DefaultTextGenerator defaultGenerator)
            {
                defaultGenerator.UseSeparator = value;
            }
        }
    } = false;

    public ObservableCollection<TokenizerType> TokenizerTypes { get; } = [];

    public ObservableCollection<TextGeneratorType> GeneratorTypes { get; } =
    [
        TextGeneratorType.Default
    ];

    public TokenizerType SelectedTokenizerType
    {
        get;
        set
        {
            Stage.Tokenizer = value switch
            {
                TokenizerType.BlockTokenizer => Stage.Tokenizer is not BlockTokenizer ? new BlockTokenizer() : Stage.Tokenizer,
                TokenizerType.SeparatorTokenizer => Stage.Tokenizer is not SeparatorTokenizer ? new SeparatorTokenizer() : Stage.Tokenizer,
                TokenizerType.RandomBlockTokenizer => Stage.Tokenizer is not RandomBlockTokenizer ? new RandomBlockTokenizer() : Stage.Tokenizer,
                TokenizerType.SmartTokenizer => Stage.Tokenizer is not SmartTokenizer ? new SmartTokenizer() : Stage.Tokenizer,
                _ => throw new ArgumentException("Selected creator type can't be added to stage!"),
            };

            SetProperty(ref field, value);
            UpdateTokenizerUsageProperties();
        }
    }

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
        get;
        set
        {
            Stage.Generator = value switch
            {
                TextGeneratorType.Default => Stage.Generator is not DefaultTextGenerator ? new DefaultTextGenerator() : Stage.Generator,
                _ => throw new ArgumentException("Selected generator type can't be added to stage!"),
            };

            SetProperty(ref field, value);
            UpdateGeneratorUsageProps();
        }
    }

    private void UpdateGeneratorUsageProps()
    {
        OnPropertyChanged(nameof(UsingDefaultGenerator));
    }


    public void ClearCache()
    {
        if(Stage.Tokenizer is CashTokenizer tokenizer)
        {
            tokenizer.ClearCash();
        }
    }

    public StageWrapper()
    {
        TokenizerTypes = [TokenizerType.BlockTokenizer, TokenizerType.RandomBlockTokenizer, TokenizerType.SeparatorTokenizer, TokenizerType.SmartTokenizer];

        SelectedTokenizerType = TokenizerType.BlockTokenizer;
        SelectedGeneratorType = TextGeneratorType.Default;
    }
}

public partial class WDViewModel : ObservableObject
{
    private readonly nint WindowHandle;

    public WDViewModel(nint windowHandle)
    {
        WindowHandle = windowHandle;
    }

    public string InputText { get => field; set => SetProperty(ref field, value); } = "Enter the text here...";
    public string ResultText
    {
        get;
        set
        {
            SetProperty(ref field, value);
        }
    } = "Chevosekrofelyab";

    public ObservableCollection<StageWrapper> Stages { get; } = new();
    private Task<string>? ExecuteTask;

    public void AddStage()
    {
        Stages.Add(new());
    }

    public void RemoveStage(StageWrapper wrapper)
    {
        Stages.Remove(wrapper);
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
        InputText = File.ReadAllText(file.Path);
    }

    public async void SaveFileAsync()
    {
        FileSavePicker picker = new();
        picker.DefaultFileExtension = ".txt";
        picker.FileTypeChoices["Text file"] = [".txt"];

        WinRT.Interop.InitializeWithWindow.Initialize(picker, WindowHandle);

        StorageFile file = await picker.PickSaveFileAsync();
        await File.WriteAllTextAsync(file.Path, ResultText);
    }
}