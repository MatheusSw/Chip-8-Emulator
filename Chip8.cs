using Chip8.Models;

namespace Chip8;

public class Chip8
{
    public readonly List<Register> _registers;
    public readonly Register _addressRegister = new() {Name = "Address"};

    public readonly Stack<int> _stack;
    public char[] Memory = new char[4096];
    public int ProgramCounter = 0;
    public int DelayTimer = 0;
    public int SoundTimer = 0;

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
                var subAddress = operand;
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
            case 0x9000:
            {
                var register = operand >> 8;
                var compRegister = operand >> 4 & 0xF;
                if (FindRegisterFromInstruction(register)!.Data
                    != FindRegisterFromInstruction(compRegister)!.Data)
                {
                    //Set skip
                }
                break;
            }
            case 0xA000:
            {
                _addressRegister.Data = operand;
                break;
            }
            case 0xB000:
            {
                _addressRegister.Data = operand + _registers.Find(r => r.Name == "V0")!.Data;
                break;
            }
            case 0xC000:
            {
                var number = operand & 0xFF;
                var register = operand >> 8;
                FindRegisterFromInstruction(register)!.Data = new Random().Next(0, 256) & number;
                break;
            }
            case 0xD000:
            {
                //Display
                break;
            }
            case 0xE09E:
            {
                
                break;
            }
            case 0xE0A1: break;
            case 0xF007:
            {
                var register = operand >> 8;
                FindRegisterFromInstruction(register)!.Data = DelayTimer;
                break;
            }
            case 0xF00A:
            {
                break;
            }
            case 0xF015:
            {
                var register = operand >> 8;
                DelayTimer = FindRegisterFromInstruction(register)!.Data;
                break;
            }
            case 0xF018:
            {
                var register = operand >> 8;
                SoundTimer = FindRegisterFromInstruction(register)!.Data;
                break;
            }
            case 0xF01E:
            {
                var register = operand >> 8;
                _addressRegister.Data += FindRegisterFromInstruction(register)!.Data;
                break;
            }
            case 0xF029:
            {
                //Sprite
                break;
            }
            case 0xF033:
            {
                var register = operand >> 8;

                Memory[ _addressRegister.Data ] = (char)(FindRegisterFromInstruction(register)!.Data / 100);
                Memory[ _addressRegister.Data + 1 ] = (char) (FindRegisterFromInstruction(register)!.Data / 10 % 10);
                Memory[ _addressRegister.Data + 2 ] = (char) (FindRegisterFromInstruction(register)!.Data % 100 % 10);
                //Thanks JamesGriffin
                break;
            }
            case 0xF055:
            {
                var registerLimit = operand >> 8;
                for (var i = 0; i <= registerLimit; i++)
                {
                    Memory[_addressRegister.Data + i] = (char)FindRegisterFromInstruction(i)!.Data;
                }
                break;
            }
            case 0xF065:
            {
                var registerLimit = operand >> 8;
                for (var i = 0; i <= registerLimit; i++)
                {
                    FindRegisterFromInstruction(i)!.Data = Memory[_addressRegister.Data + i];
                }
                break;
            }
        }
    }
}