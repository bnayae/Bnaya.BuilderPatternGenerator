#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
using Bnaya.CodeGeneration.BuilderPatternGeneration;

namespace Bnaya.BuilderPatternGenerator.SrcGen.Playground;

[GenerateBuilderPattern]
public partial struct Struct2
{
    public Struct2(int value, string name)
    {
        Value = value;
        Name = name;
    }

    public required int Quantity { get; init; }
    public string Tag { get; init; } = string.Empty;

    public string Description => $"{Name}: {Quantity}";

    public int Value { get; }
    public string Name { get; }
}
