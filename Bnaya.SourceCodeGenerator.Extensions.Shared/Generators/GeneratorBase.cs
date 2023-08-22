#pragma warning disable HAA0301 // Closure Allocation Source
#pragma warning disable S112 // General exceptions should never be thrown
#pragma warning disable HAA0603 // Delegate allocation from a method group
#pragma warning disable HAA0401 // Possible allocation of reference type enumerator

using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp.Syntax;

public abstract class GeneratorBase : IIncrementalGenerator
{
    /// <summary>
    /// Predicates for the code generation activation term.
    /// </summary>
    /// <param name="syntaxNode">The syntax node.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    protected abstract bool Predicate(SyntaxNode syntaxNode, CancellationToken cancellationToken);

    #region Initialize

    /// <summary>
    /// Called to initialize the generator and register generation steps via callbacks
    /// on the <paramref name="context" />
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.CodeAnalysis.IncrementalGeneratorInitializationContext" /> to register callbacks on</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<GenerationInput> classDeclarations =
                context.SyntaxProvider
                    .CreateSyntaxProvider(
                        predicate: Predicate,
                        transform: static (ctx, _) => ToGenerationInput(ctx))
                    .Where(static m => m is not null);

        IncrementalValueProvider<(Compilation, ImmutableArray<GenerationInput>)> compilationAndClasses
            = context.CompilationProvider.Combine(classDeclarations.Collect());

        // register a code generator for the triggers
        context.RegisterSourceOutput(compilationAndClasses, Generate);

        static GenerationInput ToGenerationInput(GeneratorSyntaxContext context)
        {
            var declarationSyntax = (TypeDeclarationSyntax)context.Node;

            var symbol = context.SemanticModel.GetDeclaredSymbol(declarationSyntax);
            if (symbol is not INamedTypeSymbol namedSymbol) throw new NullReferenceException($"Code generated symbol of {nameof(declarationSyntax)} is missing");
            return new GenerationInput(declarationSyntax, namedSymbol);
        }

        void Generate(
                       SourceProductionContext spc,
                       (Compilation compilation,
                       ImmutableArray<GenerationInput> items) source)
        {
            var (compilation, items) = source;
            foreach (GenerationInput item in items)
            {
                IEnumerable<GenerationContent> generators = OnGenerate(spc, compilation, item);
                foreach (var generator in generators)
                {
                    spc.AddSource($"{generator.FileName}.cs", generator.Content);
                }
            }
        }
    }

    #endregion // Initialize

    #region OnGenerate

    /// <summary>
    /// Generator a handler interception.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="input">Reference to the code section which is the input for the generator.</param>
    protected abstract IEnumerable<GenerationContent> OnGenerate(
        SourceProductionContext context,
        Compilation compilation,
        GenerationInput input);

    #endregion // OnGenerate
}