using Chip8.Models;

namespace Chip8;

public static class Processor
{
    public static void Process(Opcode opcode)
    {
        
        switch (opcode.Name)
        {
            case "0NNN": break;
            case "000E": break;
            case "00EE": 
                
                break;
            case "1NNN": break;
            case "2NNN": break;
            case "3XNN": break;
            case "4XNN": break;
            case "5XY0": break;
            case "6XNN": break;
            case "7XNN": break;
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