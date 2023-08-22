namespace Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Base class for code generator which activate when discover a specific attribute
/// </summary>
/// <seealso cref="GeneratorBase" />
public abstract class AttributeGeneratorBase : GeneratorBase
{
    /// <summary>
    /// Gets the target attribute which trigger a code generation (don't need the 'Attribute' suffix).
    /// </summary>
    protected abstract string TargetAttribute { get; }

    /// <summary>
    /// Gets the predicate.
    /// </summary>
    protected sealed override bool Predicate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        return syntaxNode.MatchAttribute(TargetAttribute, cancellationToken);
    }
}