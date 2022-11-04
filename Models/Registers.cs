namespace Chip8.Models;

public class Registers
{
    public List<Register> registers = new();

    public Registers()
    {
        registers = new List<Register>
        {
            new Register {Name = "VO"},
            new Register {Name = "V1"},
            new Register {Name = "V2"},
            new Register {Name = "V3"},
            new Register {Name = "V4"},
            new Register {Name = "V5"},
            new Register {Name = "V6"},
            new Register {Name = "V7"},
            new Register {Name = "V8"},
            new Register {Name = "V9"},
            new Register {Name = "VA"},
            new Register {Name = "VB"},
            new Register {Name = "VC"},
            new Register {Name = "VD"},
            new Register {Name = "VE"},
            new Register {Name = "VF"}
        };
    }
}