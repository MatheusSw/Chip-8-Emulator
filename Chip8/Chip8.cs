﻿using Chip8.Models;

namespace Chip8;

public class Chip8
{
    public int RomMemoryStart = 512;

    public readonly List<Register> Registers;
    public readonly Register AddressRegister = new() {Name = "Address"};

    public readonly Stack<int> Stack;
    public char[] Memory = new char[4096];

    public int ProgramCounter = 0;

    public int DelayTimer = 0;
    public int SoundTimer = 0;
    public bool SkipFlag = false;

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

        ProgramCounter += 2;
        
        ProcessInstruction(instruction, new Opcode {Instruction = instruction});
    }

    private Register FindRegisterFromInstruction(int register)
    {
        return Registers.First(r => r.Name == $"V{register:X}");
    }

    public void ProcessInstruction(int instruction, Opcode opcode)
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
                AddressRegister.Data = opcode.Instruction & 0xFFF;
                break;
            case 0x2000:
                var subAddress = operand;
                Stack.Push(AddressRegister.Data);
                AddressRegister.Data = subAddress;
                break;
            case 0x3000:
            {
                var register = operand >> 8;
                var value = operand & 0xFF;
                if (FindRegisterFromInstruction(register).Data == value)
                {
                    SkipFlag = true;
                }

                break;
            }
            case 0x4000:
            {
                var register = operand >> 8;
                var value = operand & 0xFF;
                if (FindRegisterFromInstruction(register).Data != value)
                {
                    SkipFlag = true;
                }

                break;
            }
            case 0x5000:
            {
                var register = operand >> 8;
                var compRegister = operand >> 4 & 0xF;
                if (FindRegisterFromInstruction(register).Data
                    == FindRegisterFromInstruction(compRegister).Data)
                {
                    SkipFlag = true;
                }

                break;
            }
            case 0x6000:
            {
                var register = operand >> 8;
                var value = operand & 0xFF;
                FindRegisterFromInstruction(register).Data = value;
                break;
            }
            case 0x7000:
            {
                var register = operand >> 8;
                var value = operand & 0xFF;
                FindRegisterFromInstruction(register).Data += value;
                break;
            }
            case 0x8000:
            {
                var originRegister = operand >> 8;
                var destinationRegister = operand >> 4 & 0xF;
                FindRegisterFromInstruction(originRegister).Data =
                    FindRegisterFromInstruction(destinationRegister).Data;
                break;
            }
            case 0x8001:
            {
                var originRegister = operand >> 8;
                var destinationRegister = operand >> 4 & 0xF;
                FindRegisterFromInstruction(originRegister).Data |=
                    FindRegisterFromInstruction(destinationRegister).Data;
                break;
            }
            case 0x8002:
            {
                var originRegister = operand >> 8;
                var destinationRegister = operand >> 4 & 0xF;
                FindRegisterFromInstruction(originRegister).Data &=
                    FindRegisterFromInstruction(destinationRegister).Data;
                break;
            }
            case 0x8003:
            {
                var originRegister = operand >> 8;
                var destinationRegister = operand >> 4 & 0xF;
                FindRegisterFromInstruction(originRegister).Data ^=
                    FindRegisterFromInstruction(destinationRegister).Data;
                break;
            }
            case 0x8004:
            {
                var originRegister = operand >> 8;
                var destinationRegister = operand >> 4 & 0xF;

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
                var originRegister = operand >> 8;
                var destinationRegister = operand >> 4 & 0xF;

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
                var originRegister = operand >> 8;
                var destinationRegister = operand >> 4 & 0xF;
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
                var register = operand >> 8;
                var compRegister = operand >> 4 & 0xF;
                if (FindRegisterFromInstruction(register).Data
                    != FindRegisterFromInstruction(compRegister).Data)
                {
                    SkipFlag = true;
                }

                break;
            }
            case 0xA000:
            {
                AddressRegister.Data = operand;
                break;
            }
            case 0xB000:
            {
                AddressRegister.Data = operand + Registers.Find(r => r.Name == "V0")!.Data;
                break;
            }
            case 0xC000:
            {
                var number = operand & 0xFF;
                var register = operand >> 8;
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
                var register = operand >> 8;
                FindRegisterFromInstruction(register).Data = DelayTimer;
                break;
            }
            case 0xF00A:
            {
                break;
            }
            case 0xF015:
            {
                var register = operand >> 8;
                DelayTimer = FindRegisterFromInstruction(register).Data;
                break;
            }
            case 0xF018:
            {
                var register = operand >> 8;
                SoundTimer = FindRegisterFromInstruction(register).Data;
                break;
            }
            case 0xF01E:
            {
                var register = operand >> 8;
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
                var register = operand >> 8;

                Memory[AddressRegister.Data] = (char) (FindRegisterFromInstruction(register).Data / 100);
                Memory[AddressRegister.Data + 1] = (char) (FindRegisterFromInstruction(register).Data / 10 % 10);
                Memory[AddressRegister.Data + 2] = (char) (FindRegisterFromInstruction(register).Data % 100 % 10);
                //Thanks JamesGriffin
                break;
            }
            case 0xF055:
            {
                var registerLimit = operand >> 8;
                for (var i = 0; i <= registerLimit; i++)
                {
                    Memory[AddressRegister.Data + i] = (char) FindRegisterFromInstruction(i).Data;
                }

                break;
            }
            case 0xF065:
            {
                var registerLimit = operand >> 8;
                for (var i = 0; i <= registerLimit; i++)
                {
                    FindRegisterFromInstruction(i).Data = Memory[AddressRegister.Data + i];
                }

                break;
            }
        }
    }
}