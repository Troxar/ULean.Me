using System;

namespace func.brainfuck
{
    public class BrainfuckBasicCommands
    {
        public static void RegisterTo(IVirtualMachine vm, Func<int> read, Action<char> write)
        {
            vm.RegisterCommand('.', x => write.Invoke(Convert.ToChar(x.Memory[x.MemoryPointer])));
            vm.RegisterCommand('+', x => AddToValueInMemory(x, +1));
            vm.RegisterCommand('-', x => AddToValueInMemory(x, -1));
            vm.RegisterCommand('>', x => AddToMemoryPointer(x, +1));
            vm.RegisterCommand('<', x => AddToMemoryPointer(x, -1));
            vm.RegisterCommand(',', x => x.Memory[x.MemoryPointer] = (byte)read.Invoke());

            RegisterCommandForSymbolRange(vm, 'A', 'Z', SetToMemory);
            RegisterCommandForSymbolRange(vm, 'a', 'z', SetToMemory);
            RegisterCommandForSymbolRange(vm, '0', '9', SetToMemory);
        }

        private static void AddToValueInMemory(IVirtualMachine vm, int value)
        {
            vm.Memory[vm.MemoryPointer] = unchecked((byte)(vm.Memory[vm.MemoryPointer] + value));
        }

        private static void AddToMemoryPointer(IVirtualMachine vm, int value)
        {
            vm.MemoryPointer += value;
            if (vm.MemoryPointer == vm.Memory.Length)
                vm.MemoryPointer = 0;
            else if (vm.MemoryPointer == -1)
                vm.MemoryPointer = vm.Memory.Length - 1;
        }

        private static void RegisterCommandForSymbolRange(IVirtualMachine vm,
            char start, char end, Action<IVirtualMachine, char> action)
        {
            for (char c = start; c <= end; c++)
            {
                char tmp = c;
                vm.RegisterCommand(c, x => action(x, tmp));
            }
        }

        private static void SetToMemory(IVirtualMachine vm, char symbol)
        {
            vm.Memory[vm.MemoryPointer] = (byte)symbol;
        }
    }
}