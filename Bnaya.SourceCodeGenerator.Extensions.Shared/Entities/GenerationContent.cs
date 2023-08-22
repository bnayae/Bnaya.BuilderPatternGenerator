namespace Microsoft.CodeAnalysis.CSharp.Syntax;

public class GenerationContent
{
    public GenerationContent(string fileName, string content)
    {
        FileName = fileName;
        Content = content;
    }

    public string FileName { get; }
    public string Content { get; }
}
