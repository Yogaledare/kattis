using System;

namespace different;

internal class Program {
    private static void Main(string[] args) {
        while (true) {
            var input = Console.ReadLine();
            if (input == null) break;

            var pair = ParseLine(input);
            var result = ComputeDifference(pair);
            Console.WriteLine(result);
        }
    }


    private static Pair ParseLine(string line) {
        var tokens = line.Split(' ');
        return new Pair(long.Parse(tokens[0]), long.Parse(tokens[1]));
    }


    private static long ComputeDifference(Pair input) {
        var first = input.First;
        var second = input.Second;

        if (first > second) return first - second;

        return second - first;
    }


    private record Pair(long First, long Second);
}