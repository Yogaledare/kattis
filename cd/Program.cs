using System;
using System.Collections.Generic;

namespace cd;

internal class Program {
    private static void Main(string[] args) {
        while (true) {
            var firstLine = Console.ReadLine();
            if (firstLine == "0 0") break;

            var tokens = firstLine.Split(' ');
            var jack = int.Parse(tokens[0]);
            var jill = int.Parse(tokens[1]);

            var numDuplicates = CountDuplicates(jack, jill);
            Console.WriteLine(numDuplicates);
        }
    }


    private static int CountDuplicates(int jack, int jill) {
        var initialCapacity = Math.Min(jack, jill);
        var set = new HashSet<long>(initialCapacity);
        var numDuplicates = 0;

        for (var i = 0; i < jack; i++) {
            var line = Console.ReadLine();
            set.Add(long.Parse(line));
        }

        for (var i = 0; i < jill; i++) {
            var line = Console.ReadLine();
            if (set.Contains(long.Parse(line))) numDuplicates++;
        }

        return numDuplicates;
    }
}