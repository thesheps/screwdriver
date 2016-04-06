using NUnit.Framework;
using Screwdriver.Builders.Tests.TestClasses;

namespace Screwdriver.Builders.Tests
{
    public class GivenExtensions
    {
        [Test]
        public void WhenIUseWithExtensionMethodAndSpecifyValidPropertyAndValue_ThenIAmAbleToAnonymouslyConstructAConcreteInstance()
        {
            var test = new Person().With(p => p.FirstName = "Steve");
            Assert.That(test.FirstName, Is.EqualTo("Steve"));
        }

        [Test]
        public void WhenISetMultipleParametersWithinTheSameLambda_ThenBothAreObserved()
        {
            var test = new Person().With(p =>
            {
                p.FirstName = "Steve";
                p.Surname = "Mclaren";
            });

            Assert.That(test.FirstName, Is.EqualTo("Steve"));
            Assert.That(test.Surname, Is.EqualTo("Mclaren"));
        }

        [Test]
        public void WhenIUseWithExtensionMethodAndSpecifyMultiplePropertiesInDifferentLambdas_ThenIAmAbleToAnonymouslyConstructAConcreteInstance()
        {
            var test = new Person()
                .With(p => p.FirstName = "Steve")
                .With(p => p.Surname = "Mclaren");

            Assert.That(test.FirstName, Is.EqualTo("Steve"));
            Assert.That(test.Surname, Is.EqualTo("Mclaren"));
        }
    }
}