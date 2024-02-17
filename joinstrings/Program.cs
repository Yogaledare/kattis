using System;
using System.IO;
using System.Text;

namespace joinstrings;

class Program {
    static void Main(string[] args) {
        using (StreamReader reader = new StreamReader(Console.OpenStandardInput(), Encoding.UTF8)) {
            var firstLine = reader.ReadLine();
            int numStrings = int.Parse(firstLine);
            EfficientConcatList[] efficientConcatLists = new EfficientConcatList[numStrings];

            for (int i = 0; i < numStrings; i++) {
                var line = reader.ReadLine();
                efficientConcatLists[i] = new EfficientConcatList(line);
            }

            for (int i = 0; i < numStrings - 1; i++) {
                var line = reader.ReadLine();
                PerformOperation(line, efficientConcatLists);
            }

            foreach (var efficientConcatList in efficientConcatLists) {
                if (!efficientConcatList.IsEmpty()) {
                    Console.WriteLine(efficientConcatList.Resolve());
                }
            }
        }
    }


    private static void PerformOperation(string line, EfficientConcatList[] strings) {
        var tokens = line.Split(' ');
        var a = int.Parse(tokens[0]) - 1;
        var b = int.Parse(tokens[1]) - 1;

        strings[a].Append(strings[b]);
        strings[b].Clear();
    }


    private class EfficientConcatList {
        private class Node {
            public readonly string Value;
            public Node? Next;

            public Node(string value) {
                Value = value;
                Next = null;
            }

            public override string ToString() {
                return (Value);
            }
        }

        private Node? _head;
        private Node? _tail;


        public EfficientConcatList(string value) {
            _head = new Node(value);
            _tail = _head;
        }


        public bool IsEmpty() {
            return (_head == null);
        }


        public void Clear() {
            _head = null;
            _tail = null;
        }


        public void Append(EfficientConcatList other) {
            if (_head == null) {
                _head = other._head;
            }
            else {
                _tail!.Next = other._head;
            }

            if (other._tail != null) {
                _tail = other._tail;
            }
        }

        public string Resolve() {
            var current = _head;
            StringBuilder stringBuilder = new StringBuilder();

            while (current != null) {
                stringBuilder.Append(current.Value);
                current = current.Next;
            }

            return stringBuilder.ToString();
        }
    }
}