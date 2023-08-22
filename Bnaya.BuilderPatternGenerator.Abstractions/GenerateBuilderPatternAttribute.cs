namespace Bnaya.CodeGeneration.BuilderPatternGeneration;

/// <summary>
/// Code generation decoration of Builder (Design Pattern) generation
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class GenerateBuilderPatternAttribute : Attribute
{
}
