using Chip8.Models;

namespace Chip8;

public class Chip8
{
    private readonly List<Register> _registers;
    private readonly Register _addressRegister = new() {Name = "Address"};

    private readonly Stack<int> _stack;
    public char[] Memory = new char[4096];
    public int ProgramCounter = 0;
    public float DelayTimer = 0;
    public float SoundTimer = 0;

    public Chip8()
    {
        _registers = new List<Register>
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
        _stack = new Stack<int>();
    }

    private Register? FindRegisterFromInstruction(int register)
    {
        return _registers.Find(r => r.Name == $"V{register:X}");
    }

    public void Process(int instruction, Opcode opcode)
    {
        var operand = instruction ^ opcode.Instruction;
        switch (instruction)
        {
            case 0xE0: break;
            case 0xEE:
                var returnAddress = _stack.Pop();
                _addressRegister.Data = returnAddress;
                break;
            case 0x1000:
                _addressRegister.Data = (opcode.Instruction << 4) >> 4;
                break;
            case 0x2000:
                var subAddress = (opcode.Instruction << 4) >> 4;
                _stack.Push(_addressRegister.Data);
                _addressRegister.Data = subAddress;
                break;
            case 0x3000:
            {
                var register = operand >> 8;
                var value = operand & 0xFF;
                if (FindRegisterFromInstruction(register)!.Data == value)
                {
                    //Set skip
                }

                break;
            }
            case 0x4000:
            {
                var register = operand >> 8;
                var value = operand & 0xFF;
                if (FindRegisterFromInstruction(register)!.Data != value)
                {
                    //Set skip
                }

                break;
            }
            case 0x5000:
            {
                var register = operand >> 8;
                var compRegister = operand >> 4 & 0xF;
                if (FindRegisterFromInstruction(register)!.Data
                    == FindRegisterFromInstruction(compRegister)!.Data)
                {
                    //Set skip
                }

                break;
            }
            case 0x6000:
            {
                var register = operand >> 8;
                var value = operand & 0xFF;
                FindRegisterFromInstruction(register)!.Data = value;
                break;
            }
            case 0x7000:
            {
                var register = operand >> 8;
                var value = operand & 0xFF;
                FindRegisterFromInstruction(register)!.Data += value;
                break;
            }
            case 0x8000:
            {
                var originRegister = operand >> 8;
                var destinationRegister = operand >> 4 & 0xF;
                FindRegisterFromInstruction(originRegister)!.Data =
                    FindRegisterFromInstruction(destinationRegister)!.Data;
                break;
            }
            case 0x8001:
            {
                var originRegister = operand >> 8;
                var destinationRegister = operand >> 4 & 0xF;
                FindRegisterFromInstruction(originRegister)!.Data |=
                    FindRegisterFromInstruction(destinationRegister)!.Data;
                break;
            }            
            case 0x8002:
            {
                var originRegister = operand >> 8;
                var destinationRegister = operand >> 4 & 0xF;
                FindRegisterFromInstruction(originRegister)!.Data &=
                    FindRegisterFromInstruction(destinationRegister)!.Data;
                break;
            }
            case 0x8003:
            {
                var originRegister = operand >> 8;
                var destinationRegister = operand >> 4 & 0xF;
                FindRegisterFromInstruction(originRegister)!.Data ^=
                    FindRegisterFromInstruction(destinationRegister)!.Data;
                break;
            }
            case 0x8004:
            {
                var originRegister = operand >> 8;
                var destinationRegister = operand >> 4 & 0xF;
                //HANDLE CARRY FLAG
                FindRegisterFromInstruction(originRegister)!.Data +=
                    FindRegisterFromInstruction(destinationRegister)!.Data;
                break;
            }
            case 0x8005:
            {
                var originRegister = operand >> 8;
                var destinationRegister = operand >> 4 & 0xF;
                //HANDLE BORROW FLAG
                FindRegisterFromInstruction(originRegister)!.Data -=
                    FindRegisterFromInstruction(destinationRegister)!.Data;
                break;
            }
            case 0x8006:
            {
                break;
            }
            case 0x8007:
            {
                var originRegister = operand >> 8;
                var destinationRegister = operand >> 4 & 0xF;
                var reg1 = FindRegisterFromInstruction(originRegister)!;
                
                //HANDLE BORROW FLAG
                 reg1.Data =
                    FindRegisterFromInstruction(destinationRegister)!.Data - reg1.Data;
                break;
            }
            case 0x800E:
            {
                
                break;
            }
            case 0x9000: break;
            case 0xA000: break;
            case 0xB000: break;
            case 0xC000: break;
            case 0xD000: break;
            case 0xE09E: break;
            case 0xE0A1: break;
            case 0xF007: break;
            case 0xF00A: break;
            case 0xF015: break;
            case 0xF018: break;
            case 0xF01E: break;
            case 0xF029: break;
            case 0xF033: break;
            case 0xF055: break;
            case 0xF065: break;
        }
    }
}