#pragma warning disable HAA0301 // Closure Allocation Source
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
#pragma warning disable HAA0401 // Possible allocation of reference type enumerator
using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Bnaya.BuilderPatternGenerator.BuilderPatternGeneration;

[Generator]
public partial class BuilderPatternGenerator : AttributeGeneratorBase
{
    protected override string TargetAttribute { get; } = "GenerateBuilderPattern";

    protected override IEnumerable<GenerationContent> OnGenerate(
            SourceProductionContext context,
            Compilation compilation,
            GenerationInput input)
    {
        INamedTypeSymbol typeSymbol = input.Symbol;
        TypeDeclarationSyntax syntax = input.Syntax;
        if (!syntax.IsPartial())
            throw new InvalidCastException($"{syntax.ToFullString()}: expected to be mark as a partial");

        var cancellationToken = context.CancellationToken;
        IMethodSymbol ctor = GetConstructor();
        ImmutableArray<IParameterSymbol> ctorPrms = ctor?.Parameters ?? ImmutableArray<IParameterSymbol>.Empty;
        var parameters = ctorPrms.Select((IParameterSymbol p, int i) =>
        {
            string def = "default";
            if (p.HasExplicitDefaultValue)
            {
                var pSyntax = p.ToSyntaxNode<ParameterSyntax>(compilation, cancellationToken);
                def = pSyntax!.Default!.Value.ToFullString();
            }
            return new MemberInfo(p.Name, p.Type, !p.HasExplicitDefaultValue, i, def);
        }).ToImmutableArray();

        var ignoreList = ImmutableHashSet.CreateRange(parameters.Select(m => m.Name));

        var props = typeSymbol.GetProperties()
                              .Where(p =>
                                                    !ignoreList.Contains(p.Name) &&
                                                    !(p.IsReadOnly && p.Name == "EqualityContract") &&
                                                    !p.IsReadOnly)
                              .OrderBy(p => p.IsReadOnly ? 0 : 1)
                              .Select((p, i) =>
                              {
                                  var def = "default";
                                  var pSyntax = p.ToSyntaxNode<PropertyDeclarationSyntax>(compilation, cancellationToken);
                                  if (pSyntax?.Initializer != null)
                                  {
                                      def = pSyntax.Initializer.Value.ToFullString();
                                  }
                                  return new MemberInfo(p.Name, p.Type, p.IsRequired, i + parameters.Length, def);
                              })
                              .ToImmutableArray();

        ImmutableArray<MemberInfo> allMembers = parameters.Concat(props)
                                         .ToImmutableArray();
        var mandatoryMembers = allMembers.Where(m => m.Mandatory)
                                         .ToImmutableArray();
        var nonMandatoryMembers = allMembers.Where(m => !m.Mandatory)
                                         //.Where(p => !ignoreList.Contains(p.Name))
                                         .ToImmutableArray();

        HashSet<string> implementations = new();
        Dictionary<string, string> permutationList = new();

        yield return Decorate(GenerateInterfaces());
        yield return Decorate(GenerateBuilder());
        yield return Decorate(GeneratePartialClass());

        #region Decorate

        GenerationContent Decorate(GenerationContent generator)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"// Generated on {DateTimeOffset.UtcNow:yyyy-MM-dd}");
            builder.AppendLine($"#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.");
            builder.AppendLine($"#pragma warning disable CS0108 // hides inherited member.");
            builder.AppendLine();
            var usingLines = syntax.GetUsing();
            foreach (var line in usingLines)
            {
                builder.AppendLine(line.Trim());
            }

            var ns = typeSymbol.ContainingNamespace.ToDisplayString();
            if (ns != null)
                builder.AppendLine($"namespace {ns};");

            usingLines = syntax.GetUsingWithinNamespace();
            foreach (var line in usingLines)
            {
                builder.AppendLine(line.Trim());
            }

            var tSyntax = typeSymbol.ToSyntaxNode<TypeDeclarationSyntax>(compilation, cancellationToken);
            var addition = typeSymbol.IsRecord && typeSymbol.TypeKind == TypeKind.Struct ? " struct" : string.Empty;
            builder.AppendLine($"partial {tSyntax!.Keyword}{addition} {typeSymbol.Name}");
            builder.AppendLine("{");

            builder.AppendLine(generator.Content);

            builder.AppendLine("}");

            return new GenerationContent(generator.FileName, builder.ToString());
        }

        #endregion // Decorate

        #region GenerateInterfaces

        GenerationContent GenerateInterfaces()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"\tpublic static class BuilderIntrfaces");
            builder.AppendLine("\t{");
            builder.AppendLine("\t\t/// <summary>");
            builder.AppendLine($"\t\t/// Build an instance of {typeSymbol.Name}");
            builder.AppendLine("\t\t/// </summary>");
            builder.AppendLine($"\t\tpublic interface I{typeSymbol.Name}BuilderBuild");
            builder.AppendLine("\t\t{");
            builder.AppendLine($"\t\t\t{typeSymbol.Name} Build();");
            builder.AppendLine("\t\t}");
            builder.AppendLine();
            builder.AppendLine("\t\t/// <summary>");
            builder.AppendLine($"\t\t/// Marker for the permutation of {typeSymbol.Name} family");
            builder.AppendLine("\t\t/// </summary>");
            builder.AppendLine($"\t\tpublic interface I{typeSymbol.Name}Family");
            builder.AppendLine("\t\t{");
            builder.AppendLine("\t\t}");
            builder.AppendLine();
            foreach (var member in allMembers)
            {
                string buildBuild = member.Mandatory ? string.Empty : $", I{typeSymbol.Name}BuilderBuild";
                builder.AppendLine($"\t\tpublic interface I{typeSymbol.Name}_{member.Name}<T>: I{typeSymbol.Name}Family{buildBuild}");
                builder.AppendLine("\t\t{");
                builder.AppendLine($"\t\t\tT Add{member.Name}({member.Type} value);");
                builder.AppendLine("\t\t}");
            }
            builder.AppendLine();

            Permutations(mandatoryMembers, nonMandatoryMembers, ImmutableList<MemberInfo>.Empty);

            builder.AppendLine("}");

            return new GenerationContent($"{typeSymbol.Name}.interfaces.generated.cs", builder.ToString());

            #region string Permutations(...)

            string Permutations(
                            IImmutableList<MemberInfo> items1,
                            IImmutableList<MemberInfo> items2,
                            IImmutableList<MemberInfo> excluded)
            {
                bool isMandatory = true;
                IImmutableList<MemberInfo> items = items1;
                if (items.Count == 0)
                {
                    isMandatory = false;
                    items = items2;
                    if (items.Count == 0)
                        return $"I{typeSymbol.Name}BuilderBuild";
                }

                var excludSequence = excluded.OrderBy(m => m.Index).Select(m => m.Name);
                string concat = string.Join("_", excludSequence);
                string interfaceName = excluded.Count == 0 // first interface used 
                            ? $"I{typeSymbol.Name}Builder"
                            : $"I{typeSymbol.Name}_Exclude_{concat}_Builder";
                if (implementations.Contains(interfaceName))
                {
                    return interfaceName;
                }
                else
                {
                    implementations.Add(interfaceName);
                }

                var inheritanceList = new List<string>();
                for (int i = 0; i < items.Count; i++)
                {
                    MemberInfo item = items[i];
#pragma warning disable S1854 // Unused assignments should be removed
                    string name = item.Name;
#pragma warning restore S1854 
                    var next = items.RemoveAt(i);
                    var nextExluded = excluded.Add(item);
                    string prmutationName = string.Empty;
                    var empty = ImmutableArray<MemberInfo>.Empty;
                    if (items1.Count == 0)
                    {
                        prmutationName = Permutations(empty, next, nextExluded);
                    }
                    else
                    {
                        prmutationName = Permutations(next, items2, nextExluded);
                    }
                    string interfaceKey = $"I{typeSymbol.Name}_{name}<{prmutationName}>";
                    var type = item.Type;
                    string interfaceImp = $"BuilderIntrfaces.{prmutationName} BuilderIntrfaces.I{typeSymbol.Name}_{name}<BuilderIntrfaces.{prmutationName}>.Add{name}({type} value) => this.Add{name}(value);";
                    permutationList.Add(interfaceKey, interfaceImp);
                    inheritanceList.Add($"\t\t\t\t\t{interfaceKey}");
                }
                builder.AppendLine($"\t\tpublic interface {interfaceName}:");
                if (!isMandatory)
                {
                    builder.AppendLine($"\t\t\t\t\tI{typeSymbol.Name}BuilderBuild, ");
                }
                builder.AppendLine(string.Join(",\r\n", inheritanceList));
                builder.AppendLine("\t\t{");
                builder.AppendLine("\t\t}");
                builder.AppendLine();

                return interfaceName;
            }

            #endregion // string Permutations(...)
        }

        #endregion // GenerateInterfaces

        #region GeneratePartialClass

        GenerationContent GeneratePartialClass()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("\t/// <summary>");
            builder.AppendLine($"\t/// Create a {typeSymbol.Name} builder");
            builder.AppendLine("\t/// </summary>");
            builder.AppendLine($"\tpublic static BuilderIntrfaces.I{typeSymbol.Name}Builder CreateBuilder() => new {typeSymbol.Name}Builder();");

            return new GenerationContent($"{typeSymbol.Name}.partial.generated.cs", builder.ToString());
        }

        #endregion // GeneratePartialClass

        #region GenerateBuilder

        GenerationContent GenerateBuilder()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"\tpublic record {typeSymbol.Name}Builder: ");
            builder.AppendLine($"\t\t\t\t\tBuilderIntrfaces.I{typeSymbol.Name}BuilderBuild,");
            var inheritance = string.Join(",\r\n\t\t\t\t\t", implementations.Select(m => $"BuilderIntrfaces.{m}"));
            builder.AppendLine($"\t\t\t\t\t{inheritance}");
            builder.AppendLine("\t{");

            foreach (var member in allMembers)
            {
                builder.AppendLine($"\t\tprivate {member.Type} {member.Name} {{ get; init; }} = {member.DefaultValue};");
                builder.AppendLine($"\t\tpublic {typeSymbol.Name}Builder Add{member.Name}({member.Type} value) => this with {{ {member.Name} = value }};");
            }

            builder.AppendLine();

            foreach (var permutation in permutationList)
            {
                builder.AppendLine($"\t\t{permutation.Value}");
            }

            string ps = string.Join(", ", parameters.Select(p => $"this.{p.Name}"));
            string prps = string.Join(",\r\n\t\t\t", props.Select(p => $"{p.OriginName} = this.{p.Name}"));
            builder.AppendLine($"\t\tpublic {typeSymbol.Name} Build() => new {typeSymbol.Name}({ps})");
            builder.AppendLine("\t\t{");
            builder.AppendLine($"\t\t\t{prps}");
            builder.AppendLine("\t\t};");


            builder.AppendLine("\t}");


            return new GenerationContent($"{typeSymbol.Name}.builder.generated.cs", builder.ToString());
        }

        #endregion // GenerateBuilder

        #region GetConstructor

        IMethodSymbol GetConstructor()
        {
            var ctors = typeSymbol.GetConstructors()
                                  .Where(c =>
                                  {
                                      if (typeSymbol.IsRecord &&
                                          c.Parameters.Length == 1 &&
                                          c.Parameters[0].Type.Name == typeSymbol.Name)
                                      {
                                          return false;
                                      }
                                      return true;
                                  });
            IMethodSymbol ctor = ctors.WhereAttribute("BuilderPatternConstructorAttribute")
                             .SingleOrDefault() ??
                             ctors.SingleOrDefault(c => c.Parameters.Length != 0);
            return ctor;
        }

        #endregion // GetConstructor
    }
}