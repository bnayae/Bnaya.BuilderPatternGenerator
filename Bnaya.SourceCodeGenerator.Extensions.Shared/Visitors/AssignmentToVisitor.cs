using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Syntax walker to find property assignments.
/// It return the names of the properties which was assigned to.
/// </summary>
/// <seealso cref="Microsoft.CodeAnalysis.CSharp.CSharpSyntaxWalker" />
public class AssignmentToVisitor : CSharpSyntaxWalker
{
    private IImmutableList<string> _propertyAssignments =
                            ImmutableList<string>.Empty;
    private readonly CancellationToken _cancellationToken;

    public AssignmentToVisitor(CancellationToken cancellationToken = default)
    {
        _cancellationToken = cancellationToken;
    }

    public static IImmutableList<string> Get(
                                            MemberDeclarationSyntax declarationSyntax,
                                            CancellationToken cancellationToken = default)
    {
        AssignmentToVisitor propertyAssignmentFinder = new AssignmentToVisitor();
        propertyAssignmentFinder.Visit(declarationSyntax);
        return propertyAssignmentFinder._propertyAssignments;
    }

    public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
    {
        if (_cancellationToken.IsCancellationRequested)
            return;


        // Check if the assignment is a property assignment
        if (node.Left is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Expression is ThisExpressionSyntax)
        {
            var name = memberAccess.Name.Identifier.ValueText;
            if (!string.IsNullOrEmpty(name))
            {
                _propertyAssignments = _propertyAssignments.Add(name);
                return;
            }
        }
        else if (node.Left is IdentifierNameSyntax id)
        {
            var name = id.Identifier.ValueText;
            if (!string.IsNullOrEmpty(name))
            {
                _propertyAssignments = _propertyAssignments.Add(name);
                return;
            }
        }


        throw new NotImplementedException($"{node.Left}, is not handled");
    }
}