using System;

namespace _3dprinter;

class Program {
    static void Main(string[] args) {
        var nString = Console.ReadLine();
        var n = int.Parse(nString);
        var makePrinterDays = FindMakePrinterDays(n);
        var neededDays = makePrinterDays + 1;
        Console.WriteLine(neededDays);
    }


    private static int FindMakePrinterDays(int n) {
        int factor = 2;
        int makePrinterDays = 0;

        while (true) {
            if (Math.Pow(factor, makePrinterDays) >= n) {
                return makePrinterDays;
            }

            makePrinterDays++;
        }
    }
}

