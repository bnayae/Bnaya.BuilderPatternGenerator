using Bnaya.CodeGeneration.BuilderPatternGeneration;

namespace Bnaya.BuilderPatternGenerator.SrcGen.Playground;

[GenerateBuilderPattern]
public partial struct Struct1
{
    public Struct1(int value, string name)
    {
        Value = value;
        Name = name;
    }

    public int Value { get; }
    public string Name { get; }
}
