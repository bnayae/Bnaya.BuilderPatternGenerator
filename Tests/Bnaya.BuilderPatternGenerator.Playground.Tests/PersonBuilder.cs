using Bnaya.CodeGeneration.BuilderPatternGeneration;

namespace Bnaya.BuilderPatternGenerator.Playground.Tests;

[GenerateBuilderPattern]
public partial record PersonBuilder(int Id, string Name)
{
    public required string Email { get; init; }
    public DateTime Birthday { get; init; }
}
