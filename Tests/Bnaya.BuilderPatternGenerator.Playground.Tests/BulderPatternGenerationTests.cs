using Bnaya.BuilderPatternGenerator.SrcGen.Playground;

using Xunit;


namespace Bnaya.CodeGeneration.BuilderPatternGeneration.Tests
{
    public class BulderPatternGenerationTests
    {
        [Fact]
        public void BulderPattern_Test()
        {
            var rec1 = Rec1.CreateBuilder()
                           .AddName("Joe")
                           .AddValue(3)
                           .Build();

            Assert.Equal(rec1, new Rec1(3, "Joe"));
            var builder2 = Rec2.CreateBuilder()
                           .AddQuantity(10)
                           .AddValue(1)
                           .AddName("Marry");
            var rec21 = builder2
                           .Build();
            Assert.Equal(rec21, new Rec2(1, "Marry") { Quantity = 10 });
            var rec22 = builder2
                            .AddRate(3)
                           .Build();
            Assert.Equal(rec22, new Rec2(1, "Marry") { Quantity = 10, Rate = 3 });
            var foo = new Foo { Value = 99 };
            var rec23 = builder2
                            .AddFoo(foo)
                           .Build();
            Assert.Equal(rec23, new Rec2(1, "Marry") { Quantity = 10, Foo = foo });
        }
    }
}
