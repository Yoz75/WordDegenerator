using System;
using System.Collections.Generic;

namespace WD.Core;

/// <summary>
/// Stage of WD
/// </summary>
public struct WDGenerationStage
{
    public ITokenizer Tokenizer;
    public ITextGenerator Generator;

    /// <summary>
    /// Count of repeating this stage. When this value is n and n > 1, <see cref="WordDegenerator"/> generates text from input and then n - 1 times "feeds" result itself
    /// </summary>
    public uint RepeatTimes;
}

public sealed class WordDegenerator
{
    public readonly List<WDGenerationStage> Stages = [];

    /// <summary>
    /// Execute the processing of text
    /// </summary>
    /// <param name="input">the input text</param>
    /// <returns>processed input</returns>
    public string Execute(string input)
    {
        string result = input;

        for(int i = 0; i < Stages.Count; i++)
        {
            WDGenerationStage stage = Stages[i];

            do
            {
                var graph = stage.Tokenizer.Tokenize(result);
                result = stage.Generator.Generate(graph);

                stage.RepeatTimes--;
            } while(stage.RepeatTimes > 0);
        }

        return result;
    }
}
