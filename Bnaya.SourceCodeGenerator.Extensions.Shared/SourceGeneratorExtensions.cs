#pragma warning disable HAA0401 // Possible allocation of reference type enumerator
#pragma warning disable HAA0303 // Lambda or anonymous method in a generic method allocates a delegate instance
#pragma warning disable HAA0301 // Closure Allocation Source

using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp.Syntax;


public static class SourceGeneratorExtensions
{
    #region MatchAttribute

    /// <summary>
    /// Check if a type matches an attribute.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static bool MatchAttribute(
                            this SyntaxNode node,
                            string attributeName,
                            CancellationToken cancellationToken)
    {
        if (node is TypeDeclarationSyntax type)
            return type.MatchAttribute(attributeName, cancellationToken);
        return false;
    }

    /// <summary>
    /// Check if a type matches an attribute.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static bool MatchAttribute(
                            this TypeDeclarationSyntax type,
                            string attributeName,
                            CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return false;

        var (attributeName1, attributeName2) = RefineAttributeNames(attributeName);

        bool hasAttributes = type.AttributeLists.Any
                               (m => m.Attributes.Any(m1 =>
                               {
                                   string name = m1.Name.ToString();
                                   bool match = name == attributeName1 || name == attributeName2;
                                   return match;
                               }));
        return hasAttributes;
    }

    #endregion // MatchAttribute

    #region IsPartial

    /// <summary>
    /// Determines whether the type marked as a partial class.
    /// </summary>
    /// <param name="syntax">The syntax.</param>
    /// <returns>
    ///   <c>true</c> if the specified syntax is partial; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsPartial<T>(this T syntax)
        where T : MemberDeclarationSyntax
    {
        return syntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
    }

    #endregion // IsPartial

    #region GetConstructors

    /// <summary>
    /// Gets the constructors.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns></returns>
    public static IEnumerable<IMethodSymbol> GetConstructors<T>(this T typeSymbol)
        where T : INamespaceOrTypeSymbol
    {
        var res = typeSymbol.GetMembers()
                                    .Where(m =>
                                    {
                                        if (m is not IMethodSymbol methodSymbol)
                                            return false;
                                        return methodSymbol.MethodKind == MethodKind.Constructor;
                                    })
                                    .Cast<IMethodSymbol>();
        return res;
    }

    #endregion // GetConstructors

    #region GetNestedBaseTypesAndSelf

    /// <summary>
    /// Gets the nested base types and self.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns></returns>
    public static IEnumerable<INamedTypeSymbol> GetNestedBaseTypesAndSelf(this INamedTypeSymbol typeSymbol)
    {
        yield return typeSymbol;
        INamedTypeSymbol? baseType = typeSymbol.BaseType;
        while (baseType != null && baseType.SpecialType != SpecialType.System_Object)
        {
            yield return baseType;
            baseType = baseType.BaseType;
        }
    }

    #endregion // GetNestedBaseTypesAndSelf

    #region GetProperties

    /// <summary>
    /// Gets the properties.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="nested">if set to <c>true</c> [nested].</param>
    /// <returns></returns>
    public static IEnumerable<IPropertySymbol> GetProperties(this INamedTypeSymbol typeSymbol, bool nested = true)
    {
        if (!nested)
        {
            return typeSymbol.GetMembers()
                                    .Where(m => m is IPropertySymbol)
                                    .Cast<IPropertySymbol>();
        }

        var types = typeSymbol.GetNestedBaseTypesAndSelf();

        IEnumerable<IPropertySymbol> res = types.SelectMany(t =>
                       t.GetMembers()
                                    .Where(m => m is IPropertySymbol)
                                    .Cast<IPropertySymbol>());
        return res;
    }

    #endregion // GetProperties

    #region GetRequired

    /// <summary>
    /// Gets the required properties.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="nested">if set to <c>true</c> [nested].</param>
    /// <returns></returns>
    public static IEnumerable<IPropertySymbol> GetRequired(this INamedTypeSymbol typeSymbol, bool nested = true)
    {
        return typeSymbol.GetProperties(nested).WhereRequired();
    }

    #endregion // GetRequired

    #region WhereRequired

    /// <summary>
    /// Gets the required properties.
    /// </summary>
    /// <param name="props">The props.</param>
    /// <returns></returns>
    public static IEnumerable<IPropertySymbol> WhereRequired(this IEnumerable<IPropertySymbol> props)
    {
        IEnumerable<IPropertySymbol> res = props.Where(p => p.IsRequired);
        return res;
    }

    #endregion // WhereRequired

    #region WhereAttribute

    /// <summary>
    /// Filter collection by attribute.
    /// </summary>
    /// <param name="methodSymbols">The method symbols.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <returns></returns>
    public static IEnumerable<T> WhereAttribute<T>(
        this IEnumerable<T> methodSymbols,
        string attributeName)
        where T : ISymbol
    {
        var (attributeName1, attributeName2) = RefineAttributeNames(attributeName);
        var result = methodSymbols.Where(m => m.GetAttributes()
                    .Any(a =>
                    {
                        string? name = a?.AttributeClass?.Name;
                        return name == attributeName1 || name == attributeName2;
                    }));
        return result;
    }

    #endregion // WhereAttribute

    #region RefineAttributeNames

    private static (string attributeName1, string attributeName2) RefineAttributeNames(
                                                                        string attributeName)
    {
        int len = attributeName.LastIndexOf(".");
        if (len != -1)
            attributeName = attributeName.Substring(len + 1);

        string attributeName2 = attributeName.EndsWith("Attribute")
            ? attributeName.Substring(0, attributeName.Length - "Attribute".Length)
            : $"{attributeName}Attribute";
        return (attributeName, attributeName2);
    }

    #endregion // RefineAttributeNames

    #region GetAssignments

    /// <summary>
    /// Gets the assignments of the symbol.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="symbol">The symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static IImmutableList<string> GetAssignmentsNames<T>(
                                                                    this ISymbol symbol,
                                                                    Compilation compilation,
                                                                    CancellationToken cancellationToken = default)
                where T : MemberDeclarationSyntax
    {
        var declarationSyntax = symbol.ToSyntaxNode<T>(compilation, cancellationToken);
        if (declarationSyntax == null)
            return ImmutableList<string>.Empty;

        return AssignmentToVisitor.Get(declarationSyntax, cancellationToken);
    }

    /// <summary>
    /// Gets the assignments of the symbol.
    /// </summary>
    /// <param name="declarationSyntax">The declaration syntax.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static IImmutableList<string> GetAssignmentsNames(
                                                                this MemberDeclarationSyntax declarationSyntax,
                                                                CancellationToken cancellationToken = default)
    {
        return AssignmentToVisitor.Get(declarationSyntax, cancellationToken);
    }

    #endregion // GetAssignments

    #region GetCtorAssignments

    /// <summary>
    /// Gets the assignments of the symbol.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static IImmutableList<string> GetCtorAssignments(
                                                                    this IMethodSymbol symbol,
                                                                    Compilation compilation,
                                                                    CancellationToken cancellationToken = default)
    {
        var results = ImmutableList<string>.Empty;
        IImmutableList<IMethodSymbol> ctors = symbol.GetConstructorCallChain(compilation, cancellationToken);
        foreach (IMethodSymbol ctor in ctors)
        {
            var asignments = ctor.GetAssignmentsNames<ConstructorDeclarationSyntax>(compilation, cancellationToken);
            results = results.AddRange(asignments);
        }
        return results;
    }

    #endregion // GetCtorAssignments

    #region ToSymbol

    /// <summary>
    /// Converts to symbol.
    /// </summary>
    /// <param name="declarationSyntax">The declaration syntax.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static ISymbol? ToSymbol(this SyntaxNode declarationSyntax,
                                     Compilation compilation,
                                     CancellationToken cancellationToken = default)
    {
        SemanticModel semanticModel = compilation.GetSemanticModel(declarationSyntax.SyntaxTree);
        ISymbol? symbol = semanticModel.GetDeclaredSymbol(declarationSyntax);
        return symbol;
    }

    #endregion // ToSymbol

    #region EqualSignature

    /// <summary>
    /// Equals the signature.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns></returns>
    public static bool EqualSignature(this IMethodSymbol a, IMethodSymbol b)
    {
        var aPrms = a.Parameters;
        var bPrms = b.Parameters;
        if (aPrms.Length != bPrms.Length)
            return false;
        for (int i = 0; i < aPrms.Length; i++)
        {
            if (!SymbolEqualityComparer.Default.Equals(aPrms[i].Type, bPrms[i].Type))
                return false;
        }
        return true;
    }

    #endregion // EqualSignature

    #region ToSyntaxNode

    /// <summary>
    /// Converts to syntax-node.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static T? ToSyntaxNode<T>(this ISymbol symbol,
                                                  Compilation compilation,
                                                  CancellationToken cancellationToken = default)
        where T : SyntaxNode
    {
        SyntaxReference? syntaxReference = symbol.DeclaringSyntaxReferences.FirstOrDefault();

        if (syntaxReference == null)
            return null;

        SyntaxTree syntaxTree = syntaxReference.SyntaxTree;
        T? nodeSyntax = syntaxTree.GetRoot(cancellationToken)
            .DescendantNodes()
            .OfType<T>()
            .FirstOrDefault(node =>
            {
                SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                ISymbol? candidateSymbol = semanticModel?.GetDeclaredSymbol(node, cancellationToken);
                if (candidateSymbol == null)
                    return false;
                return SymbolEqualityComparer.Default.Equals(symbol, candidateSymbol);
            });

        return nodeSyntax;
    }

    #endregion // ToSyntaxNode

    #region ToConstructorSyntax

    /// <summary>
    /// Converts to constructor-syntax.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static ConstructorDeclarationSyntax? ToConstructorSyntax(this IMethodSymbol symbol,
                                                                    Compilation compilation,
                                                                    CancellationToken cancellationToken = default)
    {
        return symbol.ToSyntaxNode<ConstructorDeclarationSyntax>(compilation, cancellationToken);
    }

    #endregion // ToConstructorSyntax

    #region GetConstructorCallChain

    /// <summary>
    /// Gets the constructor call chain.
    /// </summary>
    /// <param name="constructorSymbol">The constructor symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static IImmutableList<IMethodSymbol> GetConstructorCallChain(
                                                this IMethodSymbol constructorSymbol,
                                                Compilation compilation,
                                                CancellationToken cancellationToken = default)
    {
        IImmutableList<IMethodSymbol> callChain = ImmutableList<IMethodSymbol>.Empty;
        if (constructorSymbol.MethodKind != MethodKind.Constructor)
            return callChain;

        callChain = callChain.Add(constructorSymbol);

        if (constructorSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is not ConstructorDeclarationSyntax constructorSyntax)
            return callChain;

        ConstructorInitializerSyntax? initializerSyntax = constructorSyntax.Initializer;
        if (initializerSyntax == null)
            return callChain;

        IMethodSymbol? calledConstructorSymbol = null;
        SyntaxTree? syntaxTree = constructorSymbol.Locations[0].SourceTree;
        if (syntaxTree == null)
            return callChain;

        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
        if (initializerSyntax.ThisOrBaseKeyword.IsKind(SyntaxKind.ThisKeyword))
        {
            // This constructor calls another constructor in the same class
            calledConstructorSymbol = constructorSymbol.ContainingType.GetMembers()
                .OfType<IMethodSymbol>()
                .FirstOrDefault(m =>
                        {
                            IEnumerable<ITypeSymbol> types = initializerSyntax.ArgumentList.Arguments.Select(arg =>
                                semanticModel.GetTypeInfo(arg.Expression, cancellationToken).Type)
                                                                    .OfType<ITypeSymbol>();
                            return m.MethodKind == MethodKind.Constructor &&
                                   m.Parameters.Select(p => p.Type)
                                               .SequenceEqual(types, SymbolEqualityComparer.Default);
                        });
        }
        else if (initializerSyntax.ThisOrBaseKeyword.IsKind(SyntaxKind.BaseKeyword))
        {
            // This constructor calls a constructor in the base class
            ITypeSymbol? baseTypeSymbol = constructorSymbol.ContainingType.BaseType;
            calledConstructorSymbol = baseTypeSymbol?.GetMembers()
                .OfType<IMethodSymbol>()
                .FirstOrDefault(m =>
                {
                    IEnumerable<ITypeSymbol> types = initializerSyntax.ArgumentList.Arguments.Select(arg =>
                        semanticModel.GetTypeInfo(arg.Expression, cancellationToken).Type)
                                                            .OfType<ITypeSymbol>();
                    return m.MethodKind == MethodKind.Constructor &&
                           m.Parameters.Select(p => p.Type)
                                       .SequenceEqual(types, SymbolEqualityComparer.Default);
                });
        }

        if (calledConstructorSymbol != null)
        {
            IImmutableList<IMethodSymbol> innerCallChain = calledConstructorSymbol.GetConstructorCallChain(compilation, cancellationToken);
            callChain = callChain.AddRange(innerCallChain);
        }

        return callChain;
    }

    #endregion // GetConstructorCallChain

    #region GetUsing

    /// <summary>
    /// Gets the using statements (declared before the namespace).
    /// </summary>
    /// <param name="syntaxNode">The syntax node.</param>
    /// <returns></returns>
    public static IEnumerable<string> GetUsing(this SyntaxNode syntaxNode)
    {
        if (syntaxNode is CompilationUnitSyntax m)
        {
            foreach (var u in m.Usings)
            {
                var match = u.ToString();
                yield return match;
            }
        }

        if (syntaxNode.Parent == null)
            yield break;

        foreach (var u in GetUsing(syntaxNode.Parent))
        {
            yield return u;
        }
    }

    #endregion // GetUsing

    #region GetUsingWithinNamespace

    /// <summary>
    /// Gets the using declared within a namespace.
    /// </summary>
    /// <param name="typeDeclaration">The type declaration.</param>
    /// <returns></returns>
    public static IImmutableList<string> GetUsingWithinNamespace(this TypeDeclarationSyntax typeDeclaration)
    {

        var fileScope = typeDeclaration.Parent! as FileScopedNamespaceDeclarationSyntax;
        return fileScope?.Usings.Select(u => u.ToFullString()).ToImmutableList() ?? ImmutableList<string>.Empty;
    }

    #endregion // GetUsingWithinNamespace
}