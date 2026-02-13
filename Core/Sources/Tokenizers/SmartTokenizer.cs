
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WD.Core.Extensions;

namespace WD.Core.Tokenizers;

/// <summary>
/// A smart pair tokenizer that joins often pairs together
/// </summary>
public sealed class SmartTokenizer : CashTokenizer
{
    [DebuggerDisplay("{Left} {Right}")]
    private readonly struct Pair
    {
        public readonly int Left, Right;

        public Pair(int left, int right)
        {
            Left = left;
            Right = right;
        }

        public static bool operator ==(Pair left, Pair right)
        {
            return left.Left == right.Left && left.Right == right.Right;
        }

        public static bool operator !=(Pair left, Pair right)
        {
            return !(left == right);
        }

        public bool Equals(Pair other) => Left == other.Left && Right == other.Right;

        public override bool Equals(object? obj) => obj is Pair other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Left, Right);
    }

    /// <summary>
    /// PriorityQueue pops the smallest item, but we want vice versa... So we made this
    /// </summary>
    private sealed class InversedComparer : IComparer<int>
    {
        public int Compare(int left, int right)
        {
            return right.CompareTo(left);
        }
    }

    public int IterationsCount
    {
        get => field;
        set
        {
            System.ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
            field = value;
        }
    } = 10;

    public int MaxTokenLength
    {
        get => field;
        set
        {
            System.ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
            field = value;
        }
    }

    public int TopK
    {
        get => field;
        set
        {
            System.ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
            field = value;
        }
    } = 3;

    protected override IGraph<VertexWeightInfo, EdgeWeightInfo> InternalTokenize(string text)
    {
        Graph<VertexWeightInfo, EdgeWeightInfo> result = [];
        Dictionary<int, string> id2Text = [];
        Dictionary<string, int> text2Id = [];
        LinkedList<int> nodes = [];

        int lastId = 0;

        foreach(char c in text)
        {
            var id = TryRegisterId(c.ToString());
            nodes.AddLast(id);
        }

        for(int i = 0; i < IterationsCount; i++)
        {
            Dictionary<Pair, int> pairCounts = [];
            PriorityQueue<Pair, int> pairs = new(new InversedComparer());

            LinkedListNode<int>? current = nodes.First;
            while(current != null && current.Next != null)
            {
                Pair pair = new(current.Value, current.Next.Value);
                TryUpdateOrAddNode(pairCounts, pairs, pair);

                current = current.Next;
            }

            foreach(var pair in pairCounts)
            {
                pairs.Enqueue(pair.Key, pair.Value);
            }

            if(pairs.Count <= 0) break;
            Pair mostFrequentPair = GetRandomFrequentPairAndRuinPairs(pairs);

            string couple = Pair2String(mostFrequentPair);
            int newId = TryRegisterId(couple);

            ConnectPair(mostFrequentPair, newId);
        }

        GenerateTokens();
        return result;

        bool TryUpdateOrAddNode(Dictionary<Pair, int> pairCounts, PriorityQueue<Pair, int> pairs, Pair pair)
        {
            if(id2Text[pair.Left].Length + id2Text[pair.Right].Length > MaxTokenLength) return false;

            if(pairCounts.TryGetValue(pair, out int pairEntersCount))
            {
                pairCounts[pair] = ++pairEntersCount;
            }
            else
            {
                pairEntersCount = 1;
                pairCounts[pair] = pairEntersCount;
            }

            return true;
        }

        int TryRegisterId(string source)
        {
            if(text2Id.TryGetValue(source, out var value))
            {
                return value;
            }
            else
            {
                var id = lastId++;
                id2Text[id] = source;
                text2Id[source] = id;
                return id;
            }
        }

        string Pair2String(Pair pair)
        {
            return id2Text[pair.Left] + id2Text[pair.Right];
        }

        void GenerateTokens()
        {
            List<Node<VertexWeightInfo, EdgeWeightInfo>> id2Node = new(id2Text.Count);

            for(int i = 0; i < id2Text.Count; i++)
            {
                VertexWeightInfo info = new();
                info.Value = id2Text[i];
                id2Node.Add(new(info));
            }

            LinkedListNode<int>? current = nodes.First;
            Node<VertexWeightInfo, EdgeWeightInfo>? graphNode = null, nextGraphNode = null;
            while(current != null && current.Next != null)
            {
                graphNode = id2Node[current.Value];
                nextGraphNode = id2Node[current.Next.Value];

                result.UpdateConnection(graphNode!, nextGraphNode!);
                current = current.Next;
            }
        }

        /// For future me: Yes, we make here pairs, witch length is greater than MaxTokenLength, but we connect only pre-chosen pair
        /// And this pair is GUARANTIED to be valid because we made a check earlier!
        void ConnectPair(Pair pair, int newId)
        {
            LinkedListNode<int>? current = nodes.First;

            while(current != null && current.Next != null)
            {
                Pair currentPair = new(current.Value, current.Next.Value);

                if(currentPair == pair)
                {
                    nodes.Remove(current.Next);
                    current.Value = newId;
                }

                current = current.Next;
            }
        }

        /// Get a random of TopK most frequent pairs and ruin the pairs queue (it removes some items)
        Pair GetRandomFrequentPairAndRuinPairs(PriorityQueue<Pair, int> pairs)
        {
            var count = pairs.Count;
            List<Pair> topKPairs = new(count);

            for(int j = 0; j < TopK && j < count; j++)
            {
                topKPairs.Add(pairs.Dequeue());
            }

            var mostFrequentPair = topKPairs[Random.Shared.Next(topKPairs.Count)];
            return mostFrequentPair;
        }
    }
}
