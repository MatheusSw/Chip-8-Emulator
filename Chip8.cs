using Chip8.Models;

namespace Chip8;

public class Chip8
{
    public List<Register> Registers;
    public Register AddressRegister = new() {Name = "Address"};

    public Stack<int> Stack;
    public char[] Memory = new char[4096];
    public int ProgramCounter = 0;
    public int DelayTimer = 0;
    public int SoundTimer = 0;

    public Chip8()
    {
        Registers = new List<Register>
        {
            new() {Name = "VO"},
            new() {Name = "V1"},
            new() {Name = "V2"},
            new() {Name = "V3"},
            new() {Name = "V4"},
            new() {Name = "V5"},
            new() {Name = "V6"},
            new() {Name = "V7"},
            new() {Name = "V8"},
            new() {Name = "V9"},
            new() {Name = "VA"},
            new() {Name = "VB"},
            new() {Name = "VC"},
            new() {Name = "VD"},
            new() {Name = "VE"},
            new() {Name = "VF"}
        };
        Stack = new Stack<int>();
    }

    public void Process(int instruction, Opcode opcode)
    {
        var operand = instruction ^ opcode.Instruction;
        switch (instruction)
        {
            case 0xE0: break;
            case 0xEE:
                var returnAddress = Stack.Pop();
                AddressRegister.Data = returnAddress;
                break;
            case 0x1000:
                AddressRegister.Data = (opcode.Instruction << 4) >> 4;
                break;
            case 0x2000:
                var subAddress = (opcode.Instruction << 4) >> 4;
                Stack.Push(AddressRegister.Data);
                AddressRegister.Data = subAddress;
                break;
            case 0x3000:
            {
                var register = operand >> 8;
                var value = operand & 0xFF;
                if (Registers.Find(r => r.Name == $"V{register:X}")!.Data == value)
                {
                    //Set skip
                }

                break;
            }
            case 0x4000:
            {
                var register = operand >> 8;
                var value = operand & 0xFF;
                if (Registers.Find(r => r.Name == $"V{register:X}")!.Data != value)
                {
                    //Set skip
                }

                break;
            }
            case 0x5000:
            {
                var register = operand >> 8;
                var compRegister = operand >> 4 & 0xF;
                if (Registers.Find(r => r.Name == $"V{register:X}")!.Data
                    == Registers.Find(r => r.Name == $"V{compRegister:X}")!.Data)
                {
                    //Set skip
                }
                break;
            }
            case 0x6000:
            {
                var register = operand >> 8;
                var value = operand & 0xFF;
                Registers.Find(r => r.Name == $"V{register:X}")!.Data = value;
                break;
            }            
            case 0x7000:
            {
                var register = operand >> 8;
                var value = operand & 0xFF;
                Registers.Find(r => r.Name == $"V{register:X}")!.Data += value;
                break;
            }            
            case "8XY0": break;
            case "8XY1": break;
            case "8XY2": break;
            case "8XY3": break;
            case "8XY4": break;
            case "8XY5": break;
            case "8XY6": break;
            case "8XY7": break;
            case "8XYE": break;
            case "9XY0": break;
            case "ANNN": break;
            case "BNNN": break;
            case "CXNN": break;
            case "DXYN": break;
            case "EX9E": break;
            case "EXA1": break;
            case "FX07": break;
            case "FX0A": break;
            case "FX15": break;
            case "FX18": break;
            case "FX1E": break;
            case "FX29": break;
            case "FX33": break;
            case "FX55": break;
            case "FX65": break;
        }
    }
}