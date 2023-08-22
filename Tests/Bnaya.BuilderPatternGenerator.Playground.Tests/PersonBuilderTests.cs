using Bnaya.BuilderPatternGenerator.Playground.Tests;

using Xunit;


namespace Bnaya.CodeGeneration.BuilderPatternGeneration.Tests;

public class PersonBuilderTests
{
    [Fact]
    public void PersonBuilder_Test()
    {
        DateTime dateTime = DateTime.Now.AddYears(-32);
        var p1 = Person.CreateBuilder()
                       .AddName("Joe")
                       .AddId(3)
                       .AddEmail("joe16272@gmail.com")
                       .AddBirthday(dateTime)
                       .Build();

        Assert.Equal(p1, new Person(3, "Joe") { Email = "joe16272@gmail.com", Birthday = dateTime });
    }
}
