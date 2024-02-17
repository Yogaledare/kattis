using System;
using System.Collections.Generic;
using System.Linq;

namespace nodup;

class Program {
    static void Main(string[] args) {
        var input = Console.ReadLine();
        var wordArray = input.Split(' ');
        var wordSet = new HashSet<string>(wordArray);
        
        var message = wordArray.Length == wordSet.Count ? "yes" : "no";
        
        Console.WriteLine(message);

        
        // var wordSet = new HashSet<string>();
        // foreach (var word in wordArray) {
            // if (!wordSet.Add(word)) {
                // Console.WriteLine("no");
                // return; 
            // }
        // }

        // Console.WriteLine("yes");
    }
}





//         
// if (string.IsNullOrEmpty(input)) {
//     Console.WriteLine("yes");
// }
//
