using NetArchTest.Rules;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Musafir.AmaduesAPI;

namespace Musafir.Test.UnitTest.ArchitectureTests
{
    [TestFixture]
    public class ArchitectureTests
    {
        [Test]
        public void Should_Not_Have_Reference_Of_UnitTest_Porject()
        {
            var webapinamesapce = "Musafir.AmaduesAPI";
            var unitTestnamespace = "Musafir.Test.UnitTest";

            var result = Types.InAssembly(typeof(Program).Assembly)
                .That()
                .ResideInNamespace(webapinamesapce)
                .ShouldNot()
                .HaveDependencyOn(unitTestnamespace)
                .GetResult();

            ClassicAssert.IsTrue(result.IsSuccessful);
        }
    }
}
