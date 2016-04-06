using System;
using NUnit.Framework;

namespace Screwdriver.Tests
{
    public class GivenTestRunner
    {
        [Test]
        public void WhenICallRun_ThenAllTestsAreExecuted()
        {
            var testRunner = new TestRunner(AppDomain.CurrentDomain.GetAssemblies());
        } 
    }
}