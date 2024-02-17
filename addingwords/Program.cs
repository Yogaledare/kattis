namespace AddingWords2;

internal class Program {
    private static void Main(string[] args) {
        var calculator = new Calculator();

        string line;
        while ((line = Console.ReadLine()) != null) calculator.ProcessOneLine(line);
    }


    public class Calculator {
        private readonly Dictionary<int, string> _numToWordDict = new();
        private readonly Dictionary<string, int> _wordToNumDict = new();


        public void RegisterToken(string name, int value) {
            _wordToNumDict[name] = value;
            _numToWordDict[value] = name;
        }


        public void ProcessOneLine(string line) {
            if (string.IsNullOrWhiteSpace(line)) return;

            var tokens = line.Split(' ');

            switch (tokens[0]) {
                case "def":
                    HandleDefinition(tokens);
                    break;
                case "calc":
                    var result = HandleCalculation(tokens);
                    Console.WriteLine(line + " " + result);
                    break;
                case "clear":
                    HandleClear();
                    break;
            }
        }

        private void HandleClear() {
            _wordToNumDict.Clear();
            _numToWordDict.Clear();
        }


        private void HandleDefinition(string[] input) {
            ValidateDefinition(input);
            var name = input[1];
            if (int.TryParse(input[2], out var value)) RegisterToken(name, value);
        }


        private string HandleCalculation(string[] input) {
            ValidateCalculation(input);
            if (!CheckIfWordsAreKnown(input)) return "unknown";

            var pairs = new Queue<OperatorOperandPair>();
            var firstOperator = ParseOperator("+");
            var firstOperand = ParseOperand(input[1]);
            pairs.Enqueue(new OperatorOperandPair(firstOperator, firstOperand));

            for (var i = 2; i < input.Length - 1; i += 2) {
                var parsedOperator = ParseOperator(input[i]);
                var parsedOperand = ParseOperand(input[i + 1]);
                pairs.Enqueue(new OperatorOperandPair(parsedOperator, parsedOperand));
            }

            var result = CalculateResult(pairs);
            var wordEquivalent = _numToWordDict.GetValueOrDefault(result, "unknown");
            return wordEquivalent;
        }


        private int CalculateResult(Queue<OperatorOperandPair> input) {
            var sum = 0;

            while (input.Count > 0) {
                var currentPair = input.Dequeue();
                var currentOperator = currentPair.Operator;
                var currentOperand = currentPair.Operand;
                sum = currentOperator(sum, currentOperand);
            }

            return sum;
        }


        private int ParseOperand(string input) {
            var parseResult = _wordToNumDict.TryGetValue(input, out var value1);

            if (!parseResult) throw new ArgumentException("could not parse ");

            return value1;
        }


        private Func<int, int, int> ParseOperator(string op) {
            switch (op) {
                case "+":
                    return (x, y) => x + y;
                case "-":
                    return (x, y) => x - y;
            }

            throw new ArgumentException("could not parse operator");
        }


        private void ValidateDefinition(string[] input) {
            if (input.Length != 3) throw new ArgumentException("definition should have 3 tokens");

            if (!int.TryParse(input[2], out var value))
                throw new ArgumentException("value could not be parsed as an integer");
        }


        private void ValidateCalculation(string[] input) {
            if (input.Length < 4) throw new ArgumentException("less than 4 arguments");

            if (input.Length % 2 != 1) throw new ArgumentException("not an odd number of tokens");

            if (input[^1] != "=") throw new ArgumentException("bad format: no '=' last");

            for (var i = 2; i < input.Length; i += 2)
                if (!"+-=".Contains(input[i]))
                    throw new ArgumentException("incorrect operator positions");
        }


        private bool CheckIfWordsAreKnown(string[] input) {
            for (var i = 1; i < input.Length; i += 2)
                if (!_wordToNumDict.TryGetValue(input[i], out var value))
                    return false;

            return true;
        }


        private record OperatorOperandPair(Func<int, int, int> Operator, int Operand);
    }
}

// string calcKeyword = @"calc";
// string variableName = @"[a-zA-Z0-9_]+";
// string operatorPattern = @"(\+|-)";
// string endEquals = @"=";
//
// string pattern =
//     "^" + calcKeyword + " " +
//     variableName + " " +
//     "(" + operatorPattern + " " + variableName + ")* " +
//     endEquals + "$";
//
// if (Regex.IsMatch())

//
// private int ApplyOperator(int input1, int input2, Func<int, int, int> function)
// {
//     // if (!_wordToNumDict.TryGetValue(input1, out int value1))
//     // {
//     //     return "unknown"; 
//     // }
//     //
//     // if (!_wordToNumDict.TryGetValue(input2, out int value2))
//     // {
//     //     return "unknown"; 
//     // }
//
//     var computationResult = function(input1, input2); 
//     var result = _numToWordDict.GetValueOrDefault(computationResult, "unknown");
//     return result; 
// }
//