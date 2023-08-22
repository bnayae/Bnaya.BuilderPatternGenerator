#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
using Bnaya.CodeGeneration.BuilderPatternGeneration;


namespace Bnaya.BuilderPatternGenerator.SrcGen.Playground;
[GenerateBuilderPattern]
public partial class Class3 : Class2
{
    public Class3(int value, string name) : base(value, name)
    {
    }

    [BuilderPatternConstructor]
    public Class3(int value, string name, string tag, bool isPrivate = false) : base(value, name, tag)
    {
        IsPrivate = isPrivate;
    }

    public bool IsPrivate { get; }
}
