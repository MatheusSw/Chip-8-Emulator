namespace Chip8.Models;

public struct Instruction
{
    public ushort Operand; //Not the right name, but whatever

    public int Opcode
    {
        get
        {
            return (Operand >> 12) switch
            {
                0x8 => Operand & 0xF00F,
                0xE => Operand & 0xF0FF,
                0xF => (Operand & 0xF0) == 0x0 ? Operand & 0xF00F :  Operand & 0xF0FF,
                0x0 => Operand & 0xFF,
                _ => Operand & 0xF000
            };
        }
    }

    public int Address => Operand & 0xFFF;
    public int FirstRegister => (Operand & 0xF00) >> 8;
    public int SecondRegister => (Operand & 0xF0) >> 4;
    public int LongConstant => Operand & 0xFF;
    public int ShortConstant => Operand & 0xF;
}