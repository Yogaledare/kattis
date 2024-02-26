using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace _10kindsofpeople;

class Program {
    static void Main(string[] args) {
        RunWithoutDiagnostics();
    }


    private static void RunWithoutDiagnostics() {
        var mapDimensionsString = Console.ReadLine();
        var tokens = mapDimensionsString.Split(' ');
        var r = int.Parse(tokens[0]);
        var c = int.Parse(tokens[1]);
        var map = ReadMap(r, c);
        var clusters = IdentifyClusters(map);
        var numQueriesString = Console.ReadLine();
        var numQueries = int.Parse(numQueriesString);
        var queries = ReadQueries(numQueries);
        CheckPaths(queries, clusters, map);
    }


    private static void CheckPaths(List<Query> queries, Dictionary<NodeId, int> clusters, bool[,] map) {
        foreach (var query in queries) {
            var node1 = query.Node1;
            var node2 = query.Node2;

            var match = (clusters[node1] == clusters[node2]);
            if (!match) {
                Console.WriteLine("neither");
                continue;
            }

            var type = map[node1.Row, node1.Col];
            switch (type) {
                case false:
                    Console.WriteLine("binary");
                    break;
                case true:
                    Console.WriteLine("decimal");
                    break;
            }
        }
    }


    private record Query(NodeId Node1, NodeId Node2);


    private record struct NodeId(int Row, int Col);


    private static List<Query> ReadQueries(int numQueries) {
        var output = new List<Query>();
        for (int i = 0; i < numQueries; i++) {
            var line = Console.ReadLine();
            var tokens = line.Split(' ');
            var r1 = int.Parse(tokens[0]) - 1;
            var c1 = int.Parse(tokens[1]) - 1;
            var r2 = int.Parse(tokens[2]) - 1;
            var c2 = int.Parse(tokens[3]) - 1;
            var node1 = new NodeId(r1, c1);
            var node2 = new NodeId(r2, c2);
            var query = new Query(node1, node2);
            output.Add(query);
        }

        return output;
    }

    private static bool[,] ReadMap(int r, int c) {
        bool[,] output = new bool[r, c];

        for (int i = 0; i < r; i++) {
            var line = Console.ReadLine();
            for (int j = 0; j < c; j++) {
                output[i, j] = line[j] == '1';
            }
        }

        return output;
    }


    private static Dictionary<NodeId, int> IdentifyClusters(bool[,] map) {
        Dictionary<NodeId, int> output = new Dictionary<NodeId, int>();
        int clusterId = 0;
        HashSet<NodeId> visited = new HashSet<NodeId>();
        // var preComputedNeighbours = PreComputeAllNeighbors(map);
        var preComputedNeighbours = new Dictionary<NodeId, List<NodeId>>();

        for (int i = 0; i < map.GetLength(0); i++) {
            for (int j = 0; j < map.GetLength(1); j++) {
                var current = new NodeId(i, j);
                if (visited.Contains(current)) {
                    continue;
                }

                visited.Add(current);
                var cluster = FindClusterWithBfs(current, map, preComputedNeighbours, visited);

                foreach (var node in cluster) {
                    output.Add(node, clusterId);
                }

                clusterId++;
                visited.UnionWith(cluster);
            }
        }

        return output;
    }


    private static HashSet<NodeId> FindClusterWithBfs(NodeId start, bool[,] map,
        Dictionary<NodeId, List<NodeId>> preComputedNeighbours, HashSet<NodeId> visited) {
        var currentType = map[start.Row, start.Col];
        var frontier = new Queue<NodeId>();
        var cluster = new HashSet<NodeId>();
        frontier.Enqueue(start);
        cluster.Add(start);

        while (frontier.Count > 0) {
            var current = frontier.Dequeue();
            // var neighbours = preComputedNeighbours[current];
            var neighbours = GetNeighbors(current, map);
            foreach (var neighbour in neighbours) {
                if (cluster.Contains(neighbour)) {
                    continue;
                }

                if (visited.Contains(neighbour)) {
                    continue;
                }

                if (map[neighbour.Row, neighbour.Col] != currentType) {
                    continue;
                }

                frontier.Enqueue(neighbour);
                cluster.Add(neighbour);
            }
        }

        return cluster;
    }


    private static List<NodeId> GetNeighbors(NodeId current, bool[,] map) {
        List<NodeId> output = new List<NodeId>();

        var currentRow = current.Row;
        var currentCol = current.Col;

        //up
        if (currentRow > 0) {
            output.Add(new NodeId(currentRow - 1, currentCol));
        }

        //down
        if (currentRow < map.GetLength(0) - 1) {
            output.Add(new NodeId(currentRow + 1, currentCol));
        }

        //left 
        if (currentCol > 0) {
            output.Add(new NodeId(currentRow, currentCol - 1));
        }

        //right
        if (currentCol < map.GetLength(1) - 1) {
            output.Add(new NodeId(currentRow, currentCol + 1));
        }

        return output;
    }


    private static Dictionary<NodeId, List<NodeId>> PreComputeAllNeighbors(bool[,] map) {
        var output = new Dictionary<NodeId, List<NodeId>>();

        for (int i = 0; i < map.GetLength(0); i++) {
            for (int j = 0; j < map.GetLength(1); j++) {
                var current = new NodeId(i, j);
                var neighbors = GetNeighbors(current, map);
                output.Add(current, neighbors);
            }
        }

        return output;
    }

    private static void RunWithDiagnostics() {
        var mapDimensionsString = Console.ReadLine();
        var tokens = mapDimensionsString.Split(' ');
        var r = int.Parse(tokens[0]);
        var c = int.Parse(tokens[1]);

        Stopwatch stopwatch = Stopwatch.StartNew();

        var map = ReadMap(r, c);

        stopwatch.Stop();
        Console.WriteLine($"ReadMap Execution Time: {stopwatch.ElapsedMilliseconds} ms");
        stopwatch.Restart();

        var clusters = IdentifyClusters(map);

        stopwatch.Stop();
        Console.WriteLine($"IdentifyClusters Execution Time: {stopwatch.ElapsedMilliseconds} ms");
        stopwatch.Restart();

        var numQueriesString = Console.ReadLine();
        var numQueries = int.Parse(numQueriesString);
        var queries = ReadQueries(numQueries);

        stopwatch.Stop();
        Console.WriteLine($"ReadQueries Execution Time: {stopwatch.ElapsedMilliseconds} ms");
        stopwatch.Restart();

        CheckPaths(queries, clusters, map);

        stopwatch.Stop();
        Console.WriteLine($"CheckPaths Execution Time: {stopwatch.ElapsedMilliseconds} ms");
    }
}