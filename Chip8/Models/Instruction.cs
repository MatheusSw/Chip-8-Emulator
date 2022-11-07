namespace Chip8.Models;

public class Instruction
{
    public ushort Self;

    public readonly Operand Operand;

    public Instruction(ushort instruction)
    {
        Self = instruction;
        
        //I could strip just the actual operand, but that would mean handling special opcodes like 0x8XXX
        Operand = new Operand
        {
            Self = instruction
        };
    }
    
    public int Opcode
    {
        get
        {
            return (Self >> 12) switch
            {
                0x8 => Self & 0xF00F,
                0xE => Self & 0xF0FF,
                0xF => (Self & 0xF0) == 0x0 ? Self & 0xF00F :  Self & 0xF0FF,
                0x0 => Self & 0xFF,
                _ => Self & 0xF000
            };
        }
    }
}