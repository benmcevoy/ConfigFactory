using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Radio7.ConfigReader.Tests
{
    public class ArrayValueTests
    {
        // can set array types
        // preset defaults are preserved

        [Fact]
        public void ArrayType_WhenSet_IsCorrect()
        {
            // arrange
            var factory = new ConfigFactory(new ConfigReaders.ConfigReader(new MockValueProvider()));

            // act
            var sut = factory.Resolve<MyArrayPoco>();

            // assert
            Assert.Equal(new[] { "setValue1", "setValue2" }, sut.MyUnSetArray);
        }

        [Fact]
        public void ArrayType_WhenAlreadyHasADefault_TheValueIsUnchanged()
        {
            // arrange
            var factory = new ConfigFactory(new ConfigReaders.ConfigReader(new MockValueProvider()));

            // act
            var sut = factory.Resolve<MyArrayPoco>();

            // assert
            Assert.Equal(new[] { "value1", "value2" }, sut.MyDefaultArray);
        }

        [Fact]
        public void ArrayType_WhenAlreadyHasADefault_TheValueIsSetFromCofig()
        {
            // arrange
            var factory = new ConfigFactory(new ConfigReaders.ConfigReader(new MockValueProvider()));

            // act
            var sut = factory.Resolve<MyArrayPoco>();

            // assert
            Assert.Equal(new[] { "setValue1", "setValue2" }, sut.MySetArray);
        }
    }

    public class MyArrayPoco
    {
        public string[] MyUnSetArray;
        public string[] MyDefaultArray = new[] { "value1", "value2" };
        public string[] MySetArray = new[] { "value1", "value2" };
    }
}
