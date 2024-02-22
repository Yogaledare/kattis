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


//
// public int Query(int e) {
//     int budget = e;
//     int profit = 0;
//
//     while (true) {
//         var result = Query(_root, budget, out var spentEffort, out var fetchedGold);
//
//         if (result == null) {
//             return profit;
//         }
//
//         if (spentEffort == null) {
//             return profit;
//         }
//
//         if (fetchedGold == null) {
//             return profit;
//         }
//
//         budget -= (int) spentEffort;
//         profit += (int) fetchedGold;
//
//         Console.WriteLine("spent: " + spentEffort + ", gold: " + fetchedGold);
//     }
// }
//
//
// private Node? Query(Node? node, int e, out int? spentEffort, out int? fetchedGold) {
//     fetchedGold = null;
//     spentEffort = null;
//
//     if (node == null) {
//         return null;
//     }
//
//     if (node.E == e) {
//         Console.WriteLine("found exact match: " + e);
//         return Extract(node, out spentEffort, out fetchedGold);
//     }
//
//     if (e > node.E) {
//         var result = Query(node.Right, e, out spentEffort, out fetchedGold);
//
//         if (spentEffort != null) {
//             node.Right = result;
//             node.Height = GetHeight(node);
//             return node;
//         }
//
//         Console.WriteLine("settling for partial match: " + e + ", node: " + node.E);
//         return Extract(node, out spentEffort, out fetchedGold);
//     }
//
//     node.Left = Query(node.Left, e, out spentEffort, out fetchedGold);
//
//     node.Height = GetHeight(node);
//     return node;
// }
//
//
// private Node? Extract(Node node, out int? spentEffort, out int? fetchedGold) {
//     spentEffort = node.E;
//     if (node.GHeap.Count > 0) {
//         fetchedGold = node.GHeap.Dequeue();
//     }
//     else {
//         throw new ArgumentException("trying to remove from empty heap");
//     }
//
//     if (node.GHeap.Count == 0) {
//         Console.WriteLine("gheap was 0");
//         return DeleteNode(node, node.E);
//     }
//
//     return node;
// }
//
//


// private interface ICommand {
//
//
//     void Execute(AvlTree tree); 
// }
//
//
// private class AddCommand : ICommand {
//
//     public int E { get; set; }
//     public int G { get; set; }
//
//     public AddCommand(int e, int g) {
//         this.E = e;
//         this.G = g; 
//     }
//
//
//     public void Execute(AvlTree tree) {
//
//         tree
//
//     }
//
// }
//


// var tree = new AvlTree();


// tree.Insert(10, 1);
// // Console.WriteLine("new insert-------------------------------------------");
// tree.Insert(5, 1);
// tree.Insert(15, 1);
// //
// tree.Insert(3, 1);
// tree.Insert(2, 1);
// tree.Insert(1, 1);
// tree.Insert(7, 1);
// //
// tree.Insert(13, 1);
// // Console.WriteLine("new insert-------------------------------------------");
//
// tree.Insert(18, 1);
// // tree.Insert(18, 1);
//
// tree.Insert(22, 1);
// // tree.Insert(25, 1);
// // tree.Insert(30, 1);
// // tree.Insert(35, 1);
// // tree.Insert(40, 1);
//
//
// tree.Insert(2, 2);
// // tree.Insert(1, 1);
// // tree.Insert(3, 3);
// tree.Insert(0, 1);
//
// Console.WriteLine();
// Console.WriteLine("finished: ");
//
// Console.WriteLine(tree.ToString());


// var r1 = tree.Query(3);
//
// Console.WriteLine("result 1: " + r1);
// Console.WriteLine();
// Console.WriteLine("finished: ");
//
// Console.WriteLine(tree.ToString());
//
// var r2 = tree.Query(0);
//
// Console.WriteLine("result 2: " + r2);
//
// Console.WriteLine();
// Console.WriteLine("finished: ");
//
// Console.WriteLine(tree.ToString());
//
// var r3 = tree.Query(19);
//
// Console.WriteLine("result 3: " + r3);
//
//
// // tree.Insert(4, 4);
// // tree.Insert(5, 5);
// // tree.Insert(6, 6);
//
//
// Console.WriteLine();
// Console.WriteLine("finished: ");
// Console.WriteLine(tree.ToString());
//
//
// var r4 = tree.Query(27);
//
// Console.WriteLine("result 4: " + r4);
//
//
// Console.WriteLine();
// Console.WriteLine("finished: ");
//
// Console.WriteLine(tree.ToString());
//
// var r5 = tree.Query(3);
//
// Console.WriteLine("result 5: " + r5);
//
//
// // tree.Insert(4, 4);
// // tree.Insert(5, 5);
// // tree.Insert(6, 6);
//
//
// Console.WriteLine();
// Console.WriteLine("finished: ");
//
// Console.WriteLine(tree.ToString());
//
//
// var r6 = tree.Query2(2);
//
// Console.WriteLine("result 6: " + r6);
//
//
// Console.WriteLine();
// Console.WriteLine("finished: ");
//
// Console.WriteLine(tree.ToString());
//
//
// var r7 = tree.Query2(15);
//
// Console.WriteLine("result 7: " + r7);
//
//
// Console.WriteLine();
// Console.WriteLine("finished: ");
//
// Console.WriteLine(tree.ToString());


// spentEffort = node.E;
// if (node.GHeap.Count > 0) {
//     fetchedGold = node.GHeap.Dequeue();
// }
// else {
//     throw new ArgumentException("trying to remove from empty heap");
// }
//
//
// if (node.GHeap.Count == 0) {
//     
//     
//     return DeleteNode(node, e);
// }
//
// return node;


// spentEffort = node.E;
// if (node.GHeap.Count > 0) {
//     Console.WriteLine("avköar inne i settling");
//     fetchedGold = node.GHeap.Dequeue();
// }
// else {
//     throw new ArgumentException("trying to remove from empty heap");
// }
//
// if (node.GHeap.Count == 0) {
//     Console.WriteLine("deletar i settling");
//     return DeleteNode(node, node.E);
// }
//
// return node;


// if (found) {
//     spentEffort = node.E;
//     if (node.GHeap.Count > 0) {
//         fetchedGold = node.GHeap.Dequeue();
//     }
//
//     if (node.GHeap.Count == 0) {
//         return DeleteNode(node, e);
//     }


// if (result != null) {
//     node.Right = result;
//     return node; 
// }
// else {
//     spentEffort = node.E;
//     fetchedGold = node.GHeap.Dequeue(); 
// }


// if (node == null) {
//     return null;
// }
//
// if (e < node.E) {
//     node.Left = QueryAndRemove(node.Left, e, out fetchedGold);
// }
// else if (e > node.E) {
//     node.Right = QueryAndRemove(node.Right, e, out fetchedGold);
// }
// else {
//     if (node.GHeap.Count > 0) {
//         fetchedGold = node.GHeap.Dequeue();
//     }
//
//     if (node.GHeap.Count == 0) {
//         DeleteNode(node, e); 
//     }
// }
//
// // do i have to update heights here? deletenode is updating heights when doing that
//


// var balance = GetBalance(node);
//
// // left
// if (balance > 1) {
//     var subBalance = GetBalance(node.Left);
//     // left-right
//     if (subBalance < 0) {
//         node.Left = RotateLeft(node.Left);
//     }
//
//     var rotatedNode = RotateRight(node);
//     return rotatedNode;
// }
//
// //right
// if (balance < -1) {
//     var subBalance = GetBalance(node.Right);
//     // right-left
//     if (subBalance > 0) {
//         node.Right = RotateRight(node.Right);
//     }
//
//     var rotatedNode = RotateLeft(node);
//     return rotatedNode;
// }
//
// return node;


//
//
// private record Quest(int Effort, int Gold);
//
//
// private class QuestHandler {
//     private SortedSet<Quest> _quests = new SortedSet<Quest>();
//
//
//     public void Add(int efford, int gold) {
//         _quests.Add(new Quest(efford, gold));
//     }
//
//
//
// }
//