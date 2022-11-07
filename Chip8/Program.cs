Console.WriteLine("Please type the rom path");
var romPath = Console.ReadLine();

if (!File.Exists(romPath))
{
    Console.WriteLine("There has been an error trying to load the rom, please check the path");
    return;
}

var chip8 = new global::Chip8.Chip8();

//You won this time C++
var bytes = File.ReadAllBytes(romPath);
for (var i = 0; i < bytes.Length; i++)
{
    chip8.Memory[chip8.RomMemoryStart + i] = (char)bytes[i];
}

while (true)
{
    chip8.Tick();
}
