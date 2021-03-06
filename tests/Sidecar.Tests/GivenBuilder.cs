﻿using NUnit.Framework;
using Sidecar.Exceptions;
using Sidecar.Tests.TestClasses;

namespace Sidecar.Tests
{
    public class GivenBuilder
    {
        [Test]
        public void WhenIUseDynamicBuilderAndCorrectPropertyNameAndValue_ThenPropertyValueIsCorrectlySet()
        {
            var builder = Builder.Create<Person>();
            builder.With
                .FirstName("Steve")
                .Surname("Mclaren");

            var person = builder.Build();
            Assert.That(person.FirstName, Is.EqualTo("Steve"));
            Assert.That(person.Surname, Is.EqualTo("Mclaren"));
        }

        [Test]
        public void WhenIUseDynamicBuilderAndIncorrectPropertyName_ThenUnknownPropertyExceptionIsThrown()
        {
            var builder = Builder.Create<Person>();
            Assert.Throws<UnknownPropertyException>(() => { builder.With.Test("Steve"); });
        }

        [Test]
        public void WhenIUseDynamicBuilderAndIncorrectPropertyType_ThenUnknownPropertyExceptionIsThrown()
        {
            var builder = Builder.Create<Person>();
            Assert.Throws<UnknownPropertyException>(() => { builder.With.FirstName(42); });
        }
    }
}