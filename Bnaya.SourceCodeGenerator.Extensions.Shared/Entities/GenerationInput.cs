namespace Microsoft.CodeAnalysis.CSharp.Syntax;

public class GenerationInput
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenerationInput"/> class.
    /// </summary>
    /// <param name="syntax">The syntax can be class or record declaration syntax.</param>
    /// <param name="symbol">The symbol.</param>
    public GenerationInput(TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
    {
        Syntax = syntax;
        Symbol = symbol;
    }

    public TypeDeclarationSyntax Syntax { get; }
    public INamedTypeSymbol Symbol { get; }
}
