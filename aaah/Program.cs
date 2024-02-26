using System;

namespace aaah;

class Program {
    static void Main(string[] args) {
        var jon = Console.ReadLine();
        var doc = Console.ReadLine();

        var jonLength = jon?.Length ?? 0;
        var docLength = doc?.Length ?? 0; 

        var message = jonLength >= docLength ? "go" : "no";

        Console.WriteLine(message);
    }
}