using System;
using System.Collections.Generic;
using System.Text;

internal class Program {
    private static void Main(string[] args) {
        var line = Console.ReadLine();

        if (int.TryParse(line, out var number)) {
            var spell = CreateSpell(number, "Abracadabra");

            foreach (var repetition in spell) Console.WriteLine(repetition);
        }
    }


    private static List<string> CreateSpell(int input, string word) {
        var output = new List<string>();
        var stringBuilder = new StringBuilder();

        for (var i = 1; i <= input; i++) {
            stringBuilder.Append(i);
            stringBuilder.Append(' ');
            stringBuilder.Append(word);
            output.Add(stringBuilder.ToString());
            stringBuilder.Clear();
        }

        return output;
    }
}