using NUnit.Framework;
using Screwdriver.Mocking.Tests.TestClasses;

namespace Screwdriver.Mocking.Tests
{
    public class GivenMock
    {
        [Test]
        public void WhenICreateAMockOfAKnownInterface_ThenMockedMethodsDontThrowNotImplementedExceptions()
        {
            var mock = Mock.Out<IMockableInterfaceA>();
            Assert.DoesNotThrow(() => mock.DoTheThing());
        }
    }
}