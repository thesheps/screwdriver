using System.Collections.Generic;
using System.Reflection;

namespace Screwdriver
{
    public class TestRunner
    {
        public TestRunner(IList<Assembly> getAssemblies)
        {
            _getAssemblies = getAssemblies;
        }

        private readonly IList<Assembly> _getAssemblies;
    }
}