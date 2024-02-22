using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kattisquest;

class Program {
    static void Main(string[] args) {
        while (true) {
            var numLinesStr = Console.ReadLine();
            if (string.IsNullOrEmpty(numLinesStr)) {
                break;
            }

            var numLines = int.Parse(numLinesStr);
            RunCommands(numLines);
        }
    }


    public static void RunCommands(int numLines) {
        var tree = new AvlTree();
        for (int i = 0; i < numLines; i++) {
            var line = Console.ReadLine();
            var tokens = line?.Split(' ') ?? Array.Empty<string>();
            if (tokens.Length <= 0) {
                return;
            }

            if (tokens[0] == "add") {
                var effort = int.Parse(tokens[1]);
                var gold = int.Parse(tokens[2]);
                tree.Insert(effort, gold);
            }

            if (tokens[0] == "query") {
                var budget = int.Parse(tokens[1]);
                var response = tree.Query2(budget);
                Console.WriteLine(response);
            }
        }
    }


    public class AvlTree {
        private Node? _root = null;

        public override string ToString() {
            return _root?.ToString() ?? "";
        }

        public void Insert(int e, int g) {
            _root = Insert(_root, e, g);
        }

        private Node Insert(Node? node, int e, int g) {
            if (node == null) {
                return new Node(e, g);
            }

            if (e < node.E) {
                node.Left = Insert(node.Left, e, g);
                // node.Left.Parent = node;
            }

            else if (e > node.E) {
                node.Right = Insert(node.Right, e, g);
                // node.Right.Parent = node;
            }

            else {
                node.GHeap.Enqueue(g, g);
                return node;
            }

            node.Height = GetHeight(node);
            var balancedNode = BalanceNode(node);
            return balancedNode;
        }


        private Node? FindHighestNodeCeil(Node? root, int e) {
            if (root == null) {
                return null;
            }

            if (e == root.E) {
                return root;
            }

            if (e > root.E) {
                var find = FindHighestNodeCeil(root.Right, e);
                return find ?? root;
            }

            return FindHighestNodeCeil(root.Left, e);
        }


        private record FetchedEntry(int SpentEffort, int FetchedGold);


        private FetchedEntry? Fetch(int e) {
            var searchResult = FindHighestNodeCeil(_root, e);

            if (searchResult == null) {
                return null;
            }

            if (searchResult.GHeap.Count <= 0) {
                throw new ArgumentException("cannot dequeue from empty heap");
            }

            var spentEffort = searchResult.E;
            var fetchedGold = searchResult.GHeap.Dequeue();

            if (searchResult.GHeap.Count == 0) {
                _root = DeleteNode(_root, spentEffort);
            }

            return new FetchedEntry(spentEffort, fetchedGold);
        }


        public int Query2(int e) {
            int budget = e;
            int profit = 0;

            while (true) {
                var result = Fetch(budget);

                if (result == null) {
                    return profit;
                }

                budget -= result.SpentEffort;
                profit += result.FetchedGold;

                // Console.WriteLine("spent: " + result.SpentEffort + ", gold: " + result.FetchedGold);
            }
        }


        private Node? DeleteNode(Node? node, int key) {
            if (node == null) {
                return null;
            }

            if (key < node.E) {
                node.Left = DeleteNode(node.Left, key);
            }
            else if (key > node.E) {
                node.Right = DeleteNode(node.Right, key);
            }
            else {
                //leaf
                if (node.Left == null && node.Right == null) {
                    return null;
                }

                // one child
                if (node.Left == null) {
                    return node.Right;
                }

                if (node.Right == null) {
                    return node.Left;
                }

                // two children
                var maxChild = FindMaxNode(node.Left);
                node.E = maxChild.E;
                node.GHeap = maxChild.GHeap;
                node.Left = DeleteNode(node.Left, maxChild.E);
            }

            node.Height = GetHeight(node);
            var balancedNode = BalanceNode(node);

            return balancedNode;
        }


        private Node BalanceNode(Node node) {
            var balance = GetBalance(node);

            // left
            if (balance > 1) {
                var subBalance = GetBalance(node.Left);

                // left-right
                if (subBalance < 0) {
                    node.Left = RotateLeft(node.Left!);
                }

                var rotatedNode = RotateRight(node);
                return rotatedNode;
            }

            //right
            if (balance < -1) {
                var subBalance = GetBalance(node.Right);

                // right-left
                if (subBalance > 0) {
                    node.Right = RotateRight(node.Right!);
                }

                var rotatedNode = RotateLeft(node);
                return rotatedNode;
            }

            return node;
        }


        private Node RotateLeft(Node node) {
            var newRoot = node.Right;

            if (newRoot == null) {
                throw new ArgumentException("cannot left rotate with empty right");
            }

            node.Right = newRoot.Left;
            newRoot.Left = node;

            node.Height = GetHeight(node);
            newRoot.Height = GetHeight(newRoot);

            return newRoot;
        }


        private Node RotateRight(Node node) {
            var newRoot = node.Left;

            if (newRoot == null) {
                throw new ArgumentException("cannot right rotate with empty left");
            }

            node.Left = newRoot.Right;
            newRoot.Right = node;

            node.Height = GetHeight(node);
            newRoot.Height = GetHeight(newRoot);

            return newRoot;
        }


        private int GetHeight(Node? node) {
            if (node == null) {
                return 0;
            }

            var leftHeight = node.Left?.Height ?? 0;
            var rightHeight = node.Right?.Height ?? 0;

            return 1 + Math.Max(leftHeight, rightHeight);
        }


        private int GetBalance(Node? node) {
            if (node == null) {
                return 0;
            }

            var leftHeight = node.Left?.Height ?? 0;
            var rightHeight = node.Right?.Height ?? 0;

            return leftHeight - rightHeight;
        }


        private Node FindMaxNode(Node node) {
            Node current = node;
            while (current.Right != null) {
                current = current.Right;
            }

            return current;
        }
    }


    private class Node {
        public int E { get; set; }
        public PriorityQueue<int, int> GHeap { get; set; }
        public Node? Left { get; set; }

        public Node? Right { get; set; }
        public int Height { get; set; }

        public Node(int e, int g) {
            E = e;
            GHeap = new PriorityQueue<int, int>(Comparer<int>.Create((x, y) => y.CompareTo(x)));
            GHeap.Enqueue(g, g);
            Height = 1;
        }


        public override string ToString() {
            return ToString(this, "", true);
        }


        private string ToString(Node? node, string indent, bool last) {
            // Base case
            if (node == null) {
                return "";
            }

            // Calculate gold sum
            int goldSum = node.GHeap.UnorderedItems.Sum(x => x.Element);

            // Building the string
            StringBuilder sb = new StringBuilder();
            sb.Append(indent);
            if (last) {
                sb.Append("R----");
                indent += "     ";
            }
            else {
                sb.Append("L----");
                indent += "|    ";
            }

            sb.Append($"E:{node.E}, G:{goldSum}, H:{node.Height}\n");
            sb.Append(ToString(node.Left, indent, false));
            sb.Append(ToString(node.Right, indent, true));

            return sb.ToString();
        }
    }
}
