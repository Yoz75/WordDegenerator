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
    public int RepeatTimes;

    /// <summary>
    /// The seed of used random
    /// </summary>
    public int Seed = -1;

    public WDGenerationStage()
    {

    }
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
            var random = stage.Seed == -1 ? new Random() : new Random(stage.Seed);

            do
            {
                var graph = stage.Tokenizer.Tokenize(result, random);
                result = stage.Generator.Generate(graph, random);

                stage.RepeatTimes--;
            } while(stage.RepeatTimes > 0);
        }

        return result;
    }
}
