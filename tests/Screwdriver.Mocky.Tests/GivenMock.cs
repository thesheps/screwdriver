﻿using System.Linq;
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

            Assert.That(mock.WasCalled(mock.DoTheThing).AtLeastOnce(), Is.EqualTo(true));
        }

        [Test]
        public void WhenIMockAMethodWithParametersRequiringBoxing_ThenTheMockedMethodRecordsThatItHasBeenCalledWithSpecifiedArguments()
        {
            var mock = Mock.Out<IMockableInterfaceA>();
            mock.DoTheThing(123);

            Assert.That(mock.WasCalled(mock.DoTheThing).WithParameters(123).AtLeastOnce(), Is.EqualTo(true));
        }

        [Test]
        public void WhenIMockAMethodWithBoxedParameters_ThenTheMockedMethodRecordsThatItHasBeenCalledWithSpecifiedArguments()
        {
            var mock = Mock.Out<IMockableInterfaceA>();
            const string helloWorld = "Hello World";

            mock.DoTheThing(helloWorld);

            Assert.That(mock.WasCalled(mock.DoTheThing).WithParameters(helloWorld).AtLeastOnce(), Is.EqualTo(true));
        }

        [Test]
        public void WhenIMockAMethodWithAnObjectArgument_ThenTheMockedMethodRecordsThatItHasBeenCalledWithSpecifiedArguments()
        {
            var mock = Mock.Out<IMockableInterfaceA>();
            var obj = new { Test = "Hello World" };

            mock.DoTheThing(obj);

            Assert.That(mock.WasCalled(mock.DoTheThing).WithParameters(obj).AtLeastOnce(), Is.EqualTo(true));
        }

        [Test]
        public void WhenIMockAMethodAndCallItTenTimes_ThenICanDetermineThatItWasCalledTenTimes()
        {
            var mock = Mock.Out<IMockableInterfaceA>();
            var obj = new { Test = "Hello World" };
            Enumerable.Range(0, 10).ToList().ForEach(i => mock.DoTheThing(obj));

            Assert.That(mock.WasCalled(mock.DoTheThing).WithParameters(obj).Exactly(10), Is.EqualTo(true));
        }
    }
}