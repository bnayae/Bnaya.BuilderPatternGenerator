namespace Bnaya.BuilderPatternGenerator.SrcGen.Playground;

internal static class Program
{
    private static void Main(string[] args)
    {
        var rec1 = Rec1.CreateBuilder()
                        .AddName("Joe")
                        .AddValue(3)
                        .Build();
        Console.WriteLine(rec1);
        var builder2 = Rec2.CreateBuilder()
                       .AddQuantity(10)
                       .AddValue(1)
                       .AddName("Marry");
        var rec21 = builder2
                       .Build();
        Console.WriteLine(rec21);
        var rec22 = builder2
                        .AddRate(3)
                       .Build();
        Console.WriteLine(rec22);
        var rec23 = builder2
                        .AddFoo(new Foo { Value = 99 })
                       .Build();
        Console.WriteLine(rec23);
        Console.ReadKey();

    }
}
