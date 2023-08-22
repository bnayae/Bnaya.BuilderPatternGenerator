#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
using Bnaya.CodeGeneration.BuilderPatternGeneration;

namespace Bnaya.BuilderPatternGenerator.SrcGen.Playground;

[GenerateBuilderPattern]
public partial record Rec2(int Value, string Name) : Rec1(Value, Name)
{
    public required int Quantity { get; init; }
    public int Rate { get; init; }
    public Foo Foo { get; init; } = new Foo { Value = 100 };

    public string Description => $"{Name}: {Quantity}";

}

public record Foo
{
    public int Value { get; init; }

    public override string ToString() => $"Foo value is: {Value}";
}