#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Bnaya.BuilderPatternGenerator.BuilderPatternGeneration;

[DebuggerDisplay("{Name}, {Type}, Mandatory = {Mandatory}, default = {DefaultValue}, OriginName = {OriginName}")]
public class MemberInfo
{
    public MemberInfo(string name, ITypeSymbol Type, bool Mandatory, int index = -1, string? defaultValue = null)
    {
        Name = name;
        this.Type = Type;
        this.Mandatory = Mandatory;
        Index = index;
        char first = name[0];
        if (char.IsLetter(first))
            name = $"{char.ToUpper(first)}{name.Substring(1)}";
        OriginName = name;
        DefaultValue = defaultValue;
    }

    public string Name { get; }
    public string OriginName { get; }
    public ITypeSymbol Type { get; }
    public bool Mandatory { get; }
    public int Index { get; }
    public string? DefaultValue { get; }


}
