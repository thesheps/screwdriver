using System;
using NUnit.Framework;
using Screwdriver.Assertions;
using Screwdriver.Exceptions;
using Screwdriver.Tests.Fakes;

namespace Screwdriver.Tests
{
    public class GivenLogicalAssertions : When
    {
        [Test]
        public void WhenFalseAssertionIsMade_ThenFalseExceptionIsThrown()
        {
            Assert.Throws<FalseException>(() => Then(() => 2 + 2 == 5));
        }

        [Test]
        public void WhenTrueAssertionIsMade_ThenTrueExceptionIsThrown()
        {
            Assert.Throws<TrueException>(() => Then(() => 2 + 2 == 4));
        }

        [Test]
        public void WhenThrowsAssertionIsMadeWithThrowingCode_ThenTrueExceptionIsThrown()
        {
            Assert.Throws<TrueException>(() => Throws<TestException>(() => { throw new TestException(); }));
        }

        [Test]
        public void WhenThrowsAssertionIsMadeWithoutThrowingCode_ThenFalseExceptionIsThrown()
        {
            Assert.Throws<FalseException>(() => Throws<TestException>(() => Console.WriteLine("Not happening.")));
        }

        [Test]
        public void WhenThrowsAssertionIsMadeAndDifferentExceptionIsThrown_ThenFalseExceptionIsThrown()
        {
            Assert.Throws<FalseException>(() => Throws<TestException>(() => { throw new ArgumentOutOfRangeException(); }));
        }
    }
}