using Chip8.Models;
using Xunit;

namespace Chip8.Tests;

public class ProcessorTests
{
    [Fact]
    public void Test_00EE_Should_ReturnFromASubRoutine()
    {
        //Arrange
        var chip8 = new Chip8();
        var subroutineAddress = 0x1;

        //Act
        chip8._stack.Push(subroutineAddress);
        chip8.Process(0xEE, new Opcode {Instruction = 0x00EE});

        //Assert
        Assert.True(chip8._stack.Count == 0);
        Assert.True(chip8._addressRegister.Data == subroutineAddress);
    }

    [Fact]
    public void Test_1NNN_Should_JumpToAddressNNN()
    {
        //Arrange
        var chip8 = new Chip8();
        var address = 0x1123;

        //Act
        chip8.Process(0x1000, new Opcode {Instruction = address});

        //Assert
        Assert.True(chip8._addressRegister.Data == address);
    }
    
    [Fact]
    public void Test_2NNN_Should_CallSubroutineAtNNN()
    {
        //Arrange
        var chip8 = new Chip8();
        const int address = 0x2123;
        const int stackAddress = 0x3;
        chip8._addressRegister.Data = stackAddress;
        
        //Act
        chip8.Process(0x2000, new Opcode {Instruction = address});

        //Assert
        Assert.True(chip8._addressRegister.Data == (address & 0xFFF));
        Assert.True(chip8._stack.Peek() == stackAddress);
    }
}