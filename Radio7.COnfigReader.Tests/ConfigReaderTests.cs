using System;
using Xunit;

namespace Radio7.ConfigReader.Tests
{
    public class ConfigReaderTests
    {
        // supports types
        // preserves defaults
        // can find IConfig
        // can find Attribute
        // respects type converter
        // supports property
        // supports fields
        // cannot set private

        // mock the value provider

        [Fact]
        public void Can_Convert_To_Type()
        {
            // arrange
            var factory = new ConfigFactory(new ConfigReaders.ConfigReader(new MockValueProvider()));

            // act
            var sut = factory.Resolve<MyConfigPoco>();
            
            // assert
            Assert.Equal(sut.MyStringHasNoBackingField, "MyStringHasNoBackingField_configured");
            Assert.Equal(sut.MyStringHasABackingField, "MyStringHasABackingField_configured");
            Assert.Equal(sut.MyFieldHasADefault, "MyFieldHasADefault_configured");
            Assert.Equal(sut.MyDate, new DateTime(2015, 10, 21));
            Assert.Equal(sut.MyInt, -123);
            Assert.Equal(sut.MyUInt, 123U);
            Assert.Equal(sut.MyByte, 123);
            Assert.Equal(sut.MySByte, -123);
            Assert.Equal(sut.MyShort, -123);
            Assert.Equal(sut.MyUShort, 123);
            Assert.Equal(sut.MyLong, -123);
            Assert.Equal(sut.MyULong, 123U);
            Assert.Equal(sut.MyFloat, -123.45f);
            Assert.Equal(sut.MySingle, -123.45f);
            Assert.Equal(sut.MyDouble, -123.45);
            Assert.Equal(sut.MyChar, '1');
            Assert.Equal(sut.MyBool, true);
            // object is not supported as there is no type converter
            Assert.Equal(sut.MyObject, null);
            Assert.Equal(sut.MyString, "MyString_configured");
            Assert.Equal(sut.MyDecimal, -123.45m);
        }

        [Fact]
        public void Factory_Preserves_Defaults()
        {
            // arrange
            var factory = new ConfigFactory(new ConfigReaders.ConfigReader(new NullValueProvider()));

            // act
            var sut = factory.Resolve<MyConfigPoco>();

            // assert
            Assert.Equal(sut.MyStringHasNoBackingField, null);
            Assert.Equal(sut.MyStringHasABackingField, "default");
            Assert.Equal(sut.MyFieldHasADefault, "default");
            Assert.Equal(sut.MyDate, new DateTime());
            Assert.Equal(sut.MyInt, 456);
            Assert.Equal(sut.MyUInt, 456u);
            Assert.Equal(sut.MyByte, 127);
            Assert.Equal(sut.MySByte, 127);
            Assert.Equal(sut.MyShort, 456);
            Assert.Equal(sut.MyUShort, 456);
            Assert.Equal(sut.MyLong, 456);
            Assert.Equal(sut.MyULong, 456u);
            Assert.Equal(sut.MyFloat, 456);
            Assert.Equal(sut.MySingle, 456);
            Assert.Equal(sut.MyDouble, 456);
            // default char, how unfortunate there is no Char.Empty
            Assert.Equal(sut.MyChar, '\0');
            Assert.Equal(sut.MyBool, false);
            Assert.Equal(sut.MyObject, null);
            Assert.Equal(sut.MyString, null);
            Assert.Equal(sut.MyDecimal, 0);
        }

        [Fact]
        public void Cannot_Set_Private()
        {
            // arrange
            var factory = new ConfigFactory(new ConfigReaders.ConfigReader(new MockValueProvider()));

            // act
            var sut = factory.Resolve<MyConfigPoco>();

            // assert
            Assert.Equal(sut.MySetterIsPrivate, null);
        }
    }
}




