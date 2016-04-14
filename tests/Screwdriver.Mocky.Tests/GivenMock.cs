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

        [Test]
        public void WhenIMockAMethodWithParametersRequiringBoxing_ThenTheMockedMethodRecordsThatItHasBeenCalledWithSpecifiedArguments()
        {
            var mock = Mock.Out<IMockableInterfaceA>();
            mock.DoTheThing(123);

            Assert.That(mock.WasCalled(mock.DoTheThing, 123), Is.EqualTo(true));
        }

        [Test]
        public void WhenIMockAMethodWithBoxedParameters_ThenTheMockedMethodRecordsThatItHasBeenCalledWithSpecifiedArguments()
        {
            var mock = Mock.Out<IMockableInterfaceA>();
            const string helloWorld = "Hello World";

            mock.DoTheThing(helloWorld);

            Assert.That(mock.WasCalled(mock.DoTheThing, helloWorld), Is.EqualTo(true));
        }

        [Test]
        public void WhenIMockAMethodWithAnObjectArgument_ThenTheMockedMethodRecordsThatItHasBeenCalledWithSpecifiedArguments()
        {
            var mock = Mock.Out<IMockableInterfaceA>();
            var obj = new { Test = "Hello World" };

            mock.DoTheThing(obj);

            Assert.That(mock.WasCalled(mock.DoTheThing, obj), Is.EqualTo(true));
        }
    }
}