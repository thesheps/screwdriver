using NUnit.Framework;
using Screwdriver.Mocking.Tests.TestClasses;

namespace Screwdriver.Mocking.Tests
{
    public class GivenMock
    {
        [Test]
        public void WhenIMockAKnownInterface_ThenMockedMethodsDontThrowNotImplementedExceptions()
        {
            var mock = Mock.Out<IMockableInterfaceA>();
            Assert.DoesNotThrow(() => mock.DoTheThing());
        }

        [Test]
        public void WhenIMockAKnownMethod_ThenTheMockedMethodRecordsThatItHasBeenCalled()
        {
            var mock = Mock.Out<IMockableInterfaceA>();
            mock.DoTheThing();

            Assert.That(mock.WasCalled(mock.DoTheThing), Is.EqualTo(true));
        }
    }
}