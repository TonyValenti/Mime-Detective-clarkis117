using System;
using System.Collections.Generic;
using System.Text;

namespace MimeDetective.Analyzers
{
    public sealed class DictionaryBasedTrie : IFileAnalyzer
    {
        private const ushort NullStandInValue = 256;

        //root dictionary contains the nodes with offset values
        private Dictionary<ushort, Node> Nodes { get; } = new Dictionary<ushort, Node>();

        /// <summary>
        /// Constructs an empty DictionaryBasedTrie
        /// </summary>
        public DictionaryBasedTrie()
        {

        }

        /// <summary>
        /// Constructs a DictionaryBasedTrie from an Enumerable of FileTypes
        /// </summary>
        /// <param name="types"></param>
        public DictionaryBasedTrie(IEnumerable<FileType> types)
        {
            if (types is null)
                throw new ArgumentNullException(nameof(types));

            foreach (var type in types)
            {
                Insert(type);
            }
        }

        public FileType Search(in ReadResult readResult)
        {
            FileType match = null;
            var enumerator = Nodes.GetEnumerator();

            while (match is null && enumerator.MoveNext())
            {
                Node node = enumerator.Current.Value;

                for (int i = node.Value; i < readResult.ReadLength; i++)
                {
                    Node prevNode = node;

                    if (!prevNode.Children.TryGetValue(readResult.Array[i], out node)
                        && !prevNode.Children.TryGetValue(NullStandInValue, out node))
                        break;

                    if ((object)node.Record != null)
                        match = node.Record;
                }

                if ((object)match != null)
                    break;
            }

            return match;
        }

        public void Insert(FileType type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (!Nodes.TryGetValue(type.HeaderOffset, out var offsetNode))
            {
                offsetNode = new Node(type.HeaderOffset);
                Nodes.Add(type.HeaderOffset, offsetNode);
            }

            offsetNode.Insert(type);
        }

        private sealed class Node
        {
            public readonly Dictionary<ushort, Node> Children = new Dictionary<ushort, Node>();

            //if complete node then this not null
            public FileType Record;

            public readonly ushort Value;

            public Node(ushort value)
            {
                Value = value;
            }

            public void Insert(FileType type)
            {
                int i = 0;
                ushort value = type.Header[i] ?? NullStandInValue;

                if (!Children.TryGetValue(value, out Node node))
                {
                    node = new Node(value);
                    Children.Add(value, node);
                }

                i++;

                for (; i < type.Header.Length; i++)
                {
                    value = type.Header[i] ?? NullStandInValue;

                    if (!node.Children.ContainsKey(value))
                    {
                        Node newNode = new Node(value);
                        node.Children.Add(value, newNode);
                    }

                    node = node.Children[value];
                }

                node.Record = type;
            }
        }
    }
}

