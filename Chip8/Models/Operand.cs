namespace Chip8.Models;

public class Operand
{
    public int Self;

    public int Address => Self & 0xFFF;
    public int FirstRegister => (Self & 0xF00) >> 8;
    public int SecondRegister => (Self & 0xF0) >> 4;
    public int LongConstant => Self & 0xFF;
    public int ShortConstant => Self & 0xF;
}