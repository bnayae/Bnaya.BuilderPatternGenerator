#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
using Bnaya.CodeGeneration.BuilderPatternGeneration;

namespace Bnaya.BuilderPatternGenerator.SrcGen.Playground;

[GenerateBuilderPattern]
public partial class Class2 : Class1
{
    [BuilderPatternConstructor]
    public Class2(int value, string name) : base(value, name)
    {
    }

    public Class2(int value, string name, string tag) : this(value, name)
    {
        Tag = tag;
    }

    public required int Quantity { get; init; }
    public string Tag { get; init; } = string.Empty;
    public DateTime Date { get; init; } = DateTime.UtcNow;

    public string Description => $"{Name}: {Quantity}";
}
