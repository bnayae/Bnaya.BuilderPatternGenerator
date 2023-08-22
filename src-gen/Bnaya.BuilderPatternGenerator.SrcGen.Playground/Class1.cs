using Bnaya.CodeGeneration.BuilderPatternGeneration;

namespace Bnaya.BuilderPatternGenerator.SrcGen.Playground;

[GenerateBuilderPattern]
public partial class Class1
{
    public Class1(int value, string name)
    {
        this.Value = value;
        Name = name;
    }

    public int Value { get; }
    public string Name { get; }
}
