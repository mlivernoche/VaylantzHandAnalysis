using CardSourceGenerator;
using System.Collections;

namespace VaylantzHandAnalysis;

public sealed class CardSearchNode
{
    public YGOCards.YGOCardName Name { get; }
    public CardSearchNode? Next { get; }

    public CardSearchNode(YGOCards.YGOCardName name, CardSearchNode? next)
    {
        Name = name;
        Next = next;
    }
}

public sealed class CardSearchNodeCollection : IEnumerable<CardSearchNode>
{
    private sealed class NodeComparer : IEqualityComparer<CardSearchNode>
    {
        private static int GetNodeHashCode(CardSearchNode? node)
        {
            var code = 0;

            while (node != null)
            {
                code = HashCode.Combine(code, node.Name.GetHashCode());
                node = node.Next;
            }

            return code;
        }

        bool IEqualityComparer<CardSearchNode>.Equals(CardSearchNode? x, CardSearchNode? y)
        {
            return GetNodeHashCode(x) == GetNodeHashCode(y);
        }

        int IEqualityComparer<CardSearchNode>.GetHashCode(CardSearchNode obj) => GetNodeHashCode(obj);
    }

    private HashSet<CardSearchNode> Nodes { get; }

    public CardSearchNodeCollection()
    {
        Nodes = new HashSet<CardSearchNode>(new NodeComparer());
    }

    public CardSearchNodeCollection Add(CardSearchNode? cardSearchNode)
    {
        var node = cardSearchNode;

        while(node != null)
        {
            if (node.Next != null)
            {
                Nodes.Add(node);
            }
            node = node.Next;
        }

        return this;
    }

    public CardSearchNodeCollection Add(IEnumerable<YGOCards.YGOCardName> names)
    {
        var graph = CreateSearchGraph(names);
        Add(graph);
        return this;
    }

    public IEnumerable<CardSearchNode> FindNode(YGOCards.YGOCardName cardName)
    {
        foreach(var graph in Nodes)
        {
            var node = graph;
            while(node != null)
            {
                if(node.Name == cardName)
                {
                    yield return node;
                }
            }
        }
    }

    public bool HasPathBetweenNodes(YGOCards.YGOCardName start, YGOCards.YGOCardName end)
    {
        foreach(var graph in Nodes)
        {
            if(!graph.Name.Equals(start))
            {
                continue;
            }

            var node = graph.Next;
            while(node != null)
            {
                if(node.Name.Equals(end))
                {
                    return true;
                }

                node = node.Next;
            }
        }

        return false;
    }

    public IEnumerator<CardSearchNode> GetEnumerator()
    {
        IEnumerable<CardSearchNode> nodes = Nodes;
        return nodes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        IEnumerable nodes = Nodes;
        return nodes.GetEnumerator();
    }

    private static CardSearchNode? CreateSearchGraph(IEnumerable<YGOCards.YGOCardName> names)
    {
        static CardSearchNode? CreateGraph(int currentNode, YGOCards.YGOCardName[] cardNames)
        {
            if (currentNode == cardNames.Length)
            {
                return null;
            }

            return new CardSearchNode(cardNames[currentNode], CreateGraph(currentNode + 1, cardNames));
        }

        return CreateGraph(0, names.ToArray());
    }
}
