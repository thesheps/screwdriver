using System.Collections.Generic;
using System.Reflection;

namespace Screwdriver
{
    public class TestRunner
    {
        public TestRunner(IList<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        private readonly IList<Assembly> _assemblies;
    }
}