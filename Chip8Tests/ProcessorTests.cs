using System.Linq;
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
        chip8.Stack.Push(subroutineAddress);
        chip8.Process(0xEE, new Opcode {Instruction = 0x00EE});

        //Assert
        Assert.True(chip8.Stack.Count == 0);
        Assert.True(chip8.AddressRegister.Data == subroutineAddress);
    }

    [Fact]
    public void Test_1NNN_Should_JumpToAddressNNN()
    {
        //Arrange
        var chip8 = new Chip8();
        var instruction = 0x1123;

        //Act
        chip8.Process(0x1000, new Opcode {Instruction = instruction});

        //Assert
        Assert.True(chip8.AddressRegister.Data == instruction);
    }
    
    [Fact]
    public void Test_2NNN_Should_CallSubroutineAtNNN()
    {
        //Arrange
        var chip8 = new Chip8();
        const int instruction = 0x2123;
        const int stackAddress = 0x3;
        chip8.AddressRegister.Data = stackAddress;
        
        //Act
        chip8.Process(0x2000, new Opcode {Instruction = instruction});

        //Assert
        Assert.True(chip8.AddressRegister.Data == (instruction & 0xFFF));
        Assert.True(chip8.Stack.Peek() == stackAddress);
    }
    
    [Fact]
    public void Test_3XNN_Should_SetSkipFlag()
    {
        //Arrange
        var chip8 = new Chip8();
        const int instruction = 0x3120;
        const int nnValue = 0x20;
        var register = chip8.Registers.First(r => r.Name == "V1");
        register.Data = nnValue;
        
        //Act
        chip8.Process(0x3000, new Opcode {Instruction = instruction});
        
        //Assert
        Assert.True(chip8.SkipFlag);
    }
    
    [Fact]
    public void Test_4XNN_Should_SetSkipFlag()
    {
        //Arrange
        var chip8 = new Chip8();
        const int instruction = 0x4120;

        //Act
        chip8.Process(0x4000, new Opcode {Instruction = instruction});
        
        //Assert
        Assert.True(chip8.SkipFlag);
    }
    
    [Fact]
    public void Test_5XY0_Should_SetSkipFlag()
    {
        //Arrange
        var chip8 = new Chip8();
        const int instruction = 0x5120;
        chip8.Registers.First(r => r.Name == "V1").Data = 0x20;
        chip8.Registers.First(r => r.Name == "V2").Data = 0x20;
        
        //Act
        chip8.Process(0x5000, new Opcode {Instruction = instruction});
        
        //Assert
        Assert.True(chip8.SkipFlag);
    }
    
    [Fact]
    public void Test_6XNN_Should_SetVxToNn()
    {
        //Arrange
        var chip8 = new Chip8();
        const int instruction = 0x6120;
        
        //Act
        chip8.Process(0x6000, new Opcode {Instruction = instruction});
        
        //Assert
        Assert.Equal(0x20, chip8.Registers.First(r => r.Name == "V1").Data);
    }
    
    [Fact]
    public void Test_7XNN_Should_AddNnToVx()
    {
        //Arrange
        var chip8 = new Chip8();
        chip8.Registers.First(r => r.Name == "V1").Data = 0x20;
        const int instruction = 0x7120;
        
        //Act
        chip8.Process(0x7000, new Opcode {Instruction = instruction});
        
        //Assert
        Assert.Equal(0x40, chip8.Registers.First(r => r.Name == "V1").Data);
    }

    [Fact]
    public void Test_8XY0_Should_SetVxToVy()
    {
        //Arrange
        var chip8 = new Chip8();
        chip8.Registers.First(r => r.Name == "V1").Data = 0x20;
        chip8.Registers.First(r => r.Name == "V2").Data = 0x80;
        const int instruction = 0x8120;
        
        //Act
        chip8.Process(0x8000, new Opcode {Instruction = instruction});
        
        //Assert
        Assert.Equal(chip8.Registers.First(r => r.Name == "V2").Data, chip8.Registers.First(r => r.Name == "V1").Data);
    }
    
    [Fact]
    public void Test_8XY1_Should_SetVxToVxOrVy()
    {
        //Arrange
        var chip8 = new Chip8();
        var reg1 = chip8.Registers.First(r => r.Name == "V1");
        var reg2 = chip8.Registers.First(r => r.Name == "V2");
        reg1.Data = 0x20;
        reg2.Data = 0x80;
        const int instruction = 0x8121;
        
        //Act
        chip8.Process(0x8001, new Opcode {Instruction = instruction});
        
        //Assert
        Assert.Equal(reg1.Data | reg2.Data, reg1.Data);
    }
    
    [Fact]
    public void Test_8XY2_Should_SetVxToVxAndVy()
    {
        //Arrange
        var chip8 = new Chip8();
        var reg1 = chip8.Registers.First(r => r.Name == "V1");
        reg1.Data = 0x20;
        
        var reg2 = chip8.Registers.First(r => r.Name == "V2");
        reg2.Data = 0x80;
        
        const int instruction = 0x8122;
        
        //Act
        chip8.Process(0x8002, new Opcode {Instruction = instruction});
        
        //Assert
        Assert.Equal(reg1.Data & reg2.Data, reg1.Data);
    }

    [Fact]
    public void Test_8XY3_Should_SetVxToVxXorVy()
    {
        //Arrange
        var chip8 = new Chip8();
        var reg1 = chip8.Registers.First(r => r.Name == "V1");
        reg1.Data = 0x20;
        
        var reg2 = chip8.Registers.First(r => r.Name == "V2");
        reg2.Data = 0x80;
        
        const int instruction = 0x8123;
        
        //Act
        chip8.Process(0x8003, new Opcode {Instruction = instruction});
        
        //Assert
        Assert.Equal(0x20 ^ 0x80, reg1.Data);
    }
    
    [Fact]
    public void Test_8XY4_Should_AddVyToVx()
    {
        //Arrange
        var chip8 = new Chip8();
        var reg1 = chip8.Registers.First(r => r.Name == "V1");
        reg1.Data = 0x20;
        
        var reg2 = chip8.Registers.First(r => r.Name == "V2");
        reg2.Data = 0x80;
        
        const int instruction = 0x8124;
        
        //Act
        chip8.Process(0x8004, new Opcode {Instruction = instruction});
        
        //Assert
        Assert.Equal(0x20 ^ 0x80, reg1.Data);
    }
    
    [Fact]
    public void Test_8XY5_Should_SubstractVyFromVx()
    {
        //Arrange
        var chip8 = new Chip8();
        var reg1 = chip8.Registers.First(r => r.Name == "V1");
        reg1.Data = 0x20;
        
        var reg2 = chip8.Registers.First(r => r.Name == "V2");
        reg2.Data = 0x80;
        
        const int instruction = 0x8125;
        
        //Act
        chip8.Process(0x8005, new Opcode {Instruction = instruction});
        
        //Assert
        Assert.True(chip8.Registers.First(r => r.Name == "VF").Data > 0);
        Assert.Equal(0x20 - 0x80, reg1.Data);
    }
}