using Chip8.Models;

namespace Chip8;

public class Chip8
{
    public readonly int RomMemoryStart = 512;

    public readonly List<Register> Registers;
    public readonly Register AddressRegister = new() {Name = "Address"};

    public readonly Stack<int> Stack;
    public readonly char[] Memory = new char[4096];

    public int ProgramCounter = 0;

    public int DelayTimer = 0;
    public int SoundTimer = 0;
    public bool SkipFlag = false;

    public Chip8()
    {
        Registers = Enumerable.Range(0, 16).Select(index => new Register {Name = $"V{index:X}"}).ToList();
        Stack = new Stack<int>();
        ProgramCounter = RomMemoryStart;
    }

    private ushort NextInstruction()
    {
        return (ushort) (Memory[ProgramCounter++] << 8 | Memory[ProgramCounter++]);
    }

    public void Tick()
    {
        if (SkipFlag)
        {
            ProgramCounter += 2;
            SkipFlag = false;
        }

        var instruction = NextInstruction();

        ProcessInstruction(new Instruction(instruction));
    }

    private Register FindRegisterFromInstruction(int register)
    {
        return Registers.First(r => r.Name == $"V{register:X}");
    }

    public void ProcessInstruction(Instruction instruction)
    {
        switch (instruction.Opcode)
        {
            case 0xE0: break;
            case 0xEE:
                var returnAddress = Stack.Pop();
                AddressRegister.Data = returnAddress;
                break;
            case 0x1000:
                AddressRegister.Data = instruction.Operand.Address;
                break;
            case 0x2000:
                var subAddress = instruction.Operand.Address;
                Stack.Push(AddressRegister.Data);
                AddressRegister.Data = subAddress;
                break;
            case 0x3000:
            {
                var register = instruction.Operand.FirstRegister;
                var value = instruction.Operand.LongConstant;
                if (FindRegisterFromInstruction(register).Data == value)
                {
                    SkipFlag = true;
                }

                break;
            }
            case 0x4000:
            {
                var register = instruction.Operand.FirstRegister;
                var value = instruction.Operand.LongConstant;
                if (FindRegisterFromInstruction(register).Data != value)
                {
                    SkipFlag = true;
                }

                break;
            }
            case 0x5000:
            {
                var register = instruction.Operand.FirstRegister;
                var compRegister = instruction.Operand.SecondRegister;
                if (FindRegisterFromInstruction(register).Data
                    == FindRegisterFromInstruction(compRegister).Data)
                {
                    SkipFlag = true;
                }

                break;
            }
            case 0x6000:
            {
                var register = instruction.Operand.FirstRegister;
                var value = instruction.Operand.LongConstant;
                FindRegisterFromInstruction(register).Data = value;
                break;
            }
            case 0x7000:
            {
                var register = instruction.Operand.FirstRegister;
                var value = instruction.Operand.LongConstant;
                FindRegisterFromInstruction(register).Data += value;
                break;
            }
            case 0x8000:
            {
                var originRegister = instruction.Operand.FirstRegister;
                var destinationRegister = instruction.Operand.SecondRegister;
                FindRegisterFromInstruction(originRegister).Data =
                    FindRegisterFromInstruction(destinationRegister).Data;
                break;
            }
            case 0x8001:
            {
                var originRegister = instruction.Operand.FirstRegister;
                var destinationRegister = instruction.Operand.SecondRegister;
                FindRegisterFromInstruction(originRegister).Data |=
                    FindRegisterFromInstruction(destinationRegister).Data;
                break;
            }
            case 0x8002:
            {
                var originRegister = instruction.Operand.FirstRegister;
                var destinationRegister = instruction.Operand.SecondRegister;
                FindRegisterFromInstruction(originRegister).Data &=
                    FindRegisterFromInstruction(destinationRegister).Data;
                break;
            }
            case 0x8003:
            {
                var originRegister = instruction.Operand.FirstRegister;
                var destinationRegister = instruction.Operand.SecondRegister;
                FindRegisterFromInstruction(originRegister).Data ^=
                    FindRegisterFromInstruction(destinationRegister).Data;
                break;
            }
            case 0x8004:
            {
                var originRegister = instruction.Operand.FirstRegister;
                var destinationRegister = instruction.Operand.SecondRegister;

                var vfRegister = Registers.First(r => r.Name == "VF");
                vfRegister.Data = FindRegisterFromInstruction(originRegister).Data +
                    FindRegisterFromInstruction(destinationRegister).Data > sbyte.MaxValue
                        ? 1
                        : 0;

                FindRegisterFromInstruction(originRegister).Data +=
                    FindRegisterFromInstruction(destinationRegister).Data;
                break;
            }
            case 0x8005:
            {
                var originRegister = instruction.Operand.FirstRegister;
                var destinationRegister = instruction.Operand.SecondRegister;

                var vfRegister = Registers.First(r => r.Name == "VF");
                vfRegister.Data = FindRegisterFromInstruction(originRegister).Data <
                                  FindRegisterFromInstruction(destinationRegister).Data
                    ? 1
                    : 0;

                FindRegisterFromInstruction(originRegister).Data -=
                    FindRegisterFromInstruction(destinationRegister).Data;
                break;
            }
            case 0x8006:
            {
                break;
            }
            case 0x8007:
            {
                var originRegister = instruction.Operand.FirstRegister;
                var destinationRegister = instruction.Operand.SecondRegister;
                var reg1 = FindRegisterFromInstruction(originRegister);

                var vfRegister = Registers.First(r => r.Name == "VF");
                vfRegister.Data = FindRegisterFromInstruction(originRegister).Data <
                                  FindRegisterFromInstruction(destinationRegister).Data
                    ? 1
                    : 0;
                reg1.Data =
                    FindRegisterFromInstruction(destinationRegister).Data - reg1.Data;
                break;
            }
            case 0x800E:
            {
                break;
            }
            case 0x9000:
            {
                var register = instruction.Operand.FirstRegister;
                var compRegister = instruction.Operand.SecondRegister;
                if (FindRegisterFromInstruction(register).Data
                    != FindRegisterFromInstruction(compRegister).Data)
                {
                    SkipFlag = true;
                }

                break;
            }
            case 0xA000:
            {
                AddressRegister.Data = instruction.Operand.LongConstant;
                break;
            }
            case 0xB000:
            {
                //Always V0
                AddressRegister.Data = instruction.Operand.LongConstant + Registers.Find(r => r.Name == "V0")!.Data;
                break;
            }
            case 0xC000:
            {
                var number = instruction.Operand.ShortConstant;
                var register = instruction.Operand.FirstRegister;
                FindRegisterFromInstruction(register).Data = new Random().Next(0, 256) & number;
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
                var register = instruction.Operand.FirstRegister;
                FindRegisterFromInstruction(register).Data = DelayTimer;
                break;
            }
            case 0xF00A:
            {
                break;
            }
            case 0xF015:
            {
                var register = instruction.Operand.FirstRegister;
                DelayTimer = FindRegisterFromInstruction(register).Data;
                break;
            }
            case 0xF018:
            {
                var register = instruction.Operand.FirstRegister;
                SoundTimer = FindRegisterFromInstruction(register).Data;
                break;
            }
            case 0xF01E:
            {
                var register = instruction.Operand.FirstRegister;
                AddressRegister.Data += FindRegisterFromInstruction(register).Data;
                break;
            }
            case 0xF029:
            {
                //Sprite
                break;
            }
            case 0xF033:
            {
                var register = instruction.Operand.FirstRegister;

                Memory[AddressRegister.Data] = (char) (FindRegisterFromInstruction(register).Data / 100);
                Memory[AddressRegister.Data + 1] = (char) (FindRegisterFromInstruction(register).Data / 10 % 10);
                Memory[AddressRegister.Data + 2] = (char) (FindRegisterFromInstruction(register).Data % 100 % 10);
                //Thanks JamesGriffin
                break;
            }
            case 0xF055:
            {
                var registerLimit = instruction.Operand.FirstRegister;
                for (var i = 0; i <= registerLimit; i++)
                {
                    Memory[AddressRegister.Data + i] = (char) FindRegisterFromInstruction(i).Data;
                }

                break;
            }
            case 0xF065:
            {
                var registerLimit = instruction.Operand.FirstRegister;
                for (var i = 0; i <= registerLimit; i++)
                {
                    FindRegisterFromInstruction(i).Data = Memory[AddressRegister.Data + i];
                }

                break;
            }
        }
    }
}