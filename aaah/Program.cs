using System;

namespace aaah;

class Program {
    static void Main(string[] args) {
        var jon = Console.ReadLine();
        var doc = Console.ReadLine();

        var message = jon.Length >= doc.Length ? "go" : "no";

        Console.WriteLine(message);
    }
}