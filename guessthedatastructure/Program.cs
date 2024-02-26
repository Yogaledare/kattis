using System;
using System.Collections.Generic;
using System.Linq;

namespace guessthedatastructure;

class Program {
    static void Main(string[] args) {
        while (true) {
            var line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) {
                return;
            }

            var numLines = int.Parse(line);
            var operations = ReadOperations(numLines);
            var message = DetermineStructureType(operations);
            Console.WriteLine(message);
        }
    }


    private static IOperation[] ReadOperations(int numLines) {
        IOperation[] output = new IOperation[numLines];

        for (int i = 0; i < numLines; i++) {
            var read = Console.ReadLine();
            var tokens = read?.Split(' ') ?? Array.Empty<string>();
            var type = int.Parse(tokens[0]);
            var value = int.Parse(tokens[1]);

            if (type == 1) {
                output[i] = new AddOperation(value);
            }
            else if (type == 2) {
                output[i] = new RemoveOperation(value);
            }
        }

        return output;
    }


    private static string DetermineStructureType(IOperation[] operations) {
        List<IStructure> structures = new List<IStructure>() {
            new StackStructure(),
            new QueueStructure(),
            new PriorityQueueStructure()
        };

        foreach (var operation in operations) {
            foreach (var structure in structures) {
                operation.Perform(structure);
            }
        }

        int possibleCount = structures.Count(s => s.IsPossible);
        if (possibleCount == 0) {
            return "impossible";
        }

        if (possibleCount > 1) {
            return "not sure";
        }

        var possibleStructure = structures.First(s => s.IsPossible);
        switch (possibleStructure.StructureType) {
            case StructureType.STACK:
                return "stack";
            case StructureType.QUEUE:
                return "queue";
            case StructureType.PRIORITYQUEUE:
                return "priority queue";
            default:
                throw new InvalidOperationException("unknown structure type");
        }
    }


    private enum StructureType {
        STACK,
        QUEUE,
        PRIORITYQUEUE,
    }


    private interface IStructure {
        bool IsPossible { get; }
        StructureType StructureType { get; }
        void Add(int item);
        void RemoveAndMatch(int item);
    }


    private abstract class BaseStructure : IStructure {
        public bool IsPossible { get; protected set; } = true;
        public StructureType StructureType { get; }


        public BaseStructure(StructureType structureType) {
            StructureType = structureType;
        }

        public abstract void Add(int item);

        public void RemoveAndMatch(int item) {
            if (Count() == 0) {
                IsPossible = false;
                return;
            }

            if (!IsPossible) {
                return;
            }

            var removed = RemoveItem();
            IsPossible = removed == item;
        }

        protected abstract int RemoveItem();
        protected abstract int Count();
    }


    private class StackStructure : BaseStructure, IStructure {
        private Stack<int> _stack = new Stack<int>();

        public StackStructure() : base(StructureType.STACK) {
        }

        public override void Add(int item) {
            _stack.Push(item);
        }

        protected override int RemoveItem() {
            return _stack.Pop();
        }

        protected override int Count() {
            return _stack.Count;
        }
    }


    private class QueueStructure : BaseStructure, IStructure {
        private Queue<int> _queue = new Queue<int>();

        public QueueStructure() : base(StructureType.QUEUE) {
        }

        public override void Add(int item) {
            _queue.Enqueue(item);
        }

        protected override int RemoveItem() {
            return _queue.Dequeue();
        }

        protected override int Count() {
            return _queue.Count;
        }
    }


    private class PriorityQueueStructure : BaseStructure, IStructure {
        private PriorityQueue<int, int> _priorityQueue =
            new PriorityQueue<int, int>(Comparer<int>.Create((x, y) => y.CompareTo(x)));

        public PriorityQueueStructure() : base(StructureType.PRIORITYQUEUE) {
        }

        public override void Add(int item) {
            _priorityQueue.Enqueue(item, item);
        }

        protected override int RemoveItem() {
            return _priorityQueue.Dequeue();
        }

        protected override int Count() {
            return _priorityQueue.Count;
        }
    }


    private interface IOperation {
        public void Perform(IStructure iStructure);
    }


    private class AddOperation : IOperation {
        private readonly int value;

        public AddOperation(int value) {
            this.value = value;
        }

        public void Perform(IStructure iStructure) {
            iStructure.Add(value);
        }
    }


    private class RemoveOperation : IOperation {
        private readonly int value;

        public RemoveOperation(int value) {
            this.value = value;
        }

        public void Perform(IStructure iStructure) {
            iStructure.RemoveAndMatch(value);
        }
    }
}