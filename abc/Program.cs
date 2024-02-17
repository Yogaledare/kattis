using System.Text;

namespace ABC2;

internal class Program {
    private static void Main(string[] args) {
        var firstLine = Console.ReadLine();
        var abcDictionary = CreateAbcDictionary(firstLine);

        var secondLine = Console.ReadLine();
        var outputString = ConstructOutputString(abcDictionary, secondLine);

        Console.WriteLine(outputString);
    }


    private static Dictionary<char, int> CreateAbcDictionary(string line) {
        var abc = "ABC";
        var output = new Dictionary<char, int>();
        var minHeap = new PriorityQueue<int, int>();
        var tokens = line.Split(" ");

        foreach (var token in tokens) {
            var number = int.Parse(token);
            minHeap.Enqueue(number, number);
        }

        if (minHeap.Count != 3) throw new ArgumentException("not 3 values");

        foreach (var letter in abc) {
            var value = minHeap.Dequeue();
            output[letter] = value;
        }

        return output;
    }


    private static string ConstructOutputString(Dictionary<char, int> abcDictionary, string line) {
        var stringBuilder = new StringBuilder();

        foreach (var character in line)
            if (abcDictionary.TryGetValue(character, out var num)) {
                if (stringBuilder.Length > 0) stringBuilder.Append(' ');

                stringBuilder.Append(num);
            }

        return stringBuilder.ToString();
    }
}