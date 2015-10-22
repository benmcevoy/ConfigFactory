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
        // supports enumerables

        [Theory,
            InlineData("MyStringHasNoBackingField", "MyStringHasNoBackingField_configured"),
            InlineData("MyStringHasABackingField", "MyStringHasABackingField_configured"),
            InlineData("MyFieldHasADefault", "MyFieldHasADefault_configured"),
            InlineData("MyInt", -123),
            InlineData("MyUInt", 123U),
            InlineData("MyByte", (byte)123),
            InlineData("MySByte", (sbyte)-123),
            InlineData("MyShort", (short)-123),
            InlineData("MyUShort", (ushort)123),
            InlineData("MyLong", -123L),
            InlineData("MyULong", 123UL),
            InlineData("MyFloat", -123.45F),
            InlineData("MySingle", -123.45F),
            InlineData("MyDouble", -123.45),
            InlineData("MyChar", '1'),
            InlineData("MyBool", true),
            InlineData("MyString", "MyString_configured"),
        ]
        public void Can_Convert_To_Type(string memberName, object expectedValue)
        {
            // arrange
            var factory = new ConfigFactory(new ConfigReaders.ConfigReader(new MockValueProvider()));

            // act
            var sut = factory.Resolve<MyConfigPoco>();
            var typeInfo = sut.GetType();
            var prop = typeInfo.GetProperty(memberName);
            var field = typeInfo.GetField(memberName);

            var actual = prop == null
                ? field.GetValue(sut)
                : prop.GetValue(sut);

            // assert
            Assert.Equal(expectedValue, actual);
        }

        [Theory,
            InlineData("MyStringHasNoBackingField", null),
            InlineData("MyStringHasABackingField", "default"),
            InlineData("MyFieldHasADefault", "default"),
            //InlineData("MyDate", new DateTime() ),
            InlineData("MyInt", 456),
            InlineData("MyUInt", 456U),
            InlineData("MyByte", (byte)127),
            InlineData("MySByte", (sbyte)127),
            InlineData("MyShort", (short)456),
            InlineData("MyUShort", (ushort)456U),
            InlineData("MyLong", 456L),
            InlineData("MyULong", 456UL),
            InlineData("MyFloat", 456F),
            InlineData("MySingle", 456F),
            InlineData("MyDouble", 456D),
            // default char, how unfortunate there is no Char.Empty
            InlineData("MyChar", '\0'),
            InlineData("MyBool", false),
            InlineData("MyString", null)]
        public void Factory_Preserves_Defaults(string memberName, object expectedValue)
        {
            // arrange
            var factory = new ConfigFactory(new ConfigReaders.ConfigReader(new NullValueProvider()));

            // act
            var sut = factory.Resolve<MyConfigPoco>();
            var typeInfo = sut.GetType();
            var prop = typeInfo.GetProperty(memberName);
            var field = typeInfo.GetField(memberName);

            var actual = prop == null
                ? field.GetValue(sut)
                : prop.GetValue(sut);

            // assert
            Assert.Equal(expectedValue, actual);
        }

        [Fact]
        public void Object_Is_Never_Set()
        {
            // arrange
            var factory = new ConfigFactory(new ConfigReaders.ConfigReader(new MockValueProvider()));

            // act
            var sut = factory.Resolve<MyConfigPoco>();

            // assert
            Assert.Equal(null, sut.MyObject);
        }

        [Fact]
        public void Decimal_Can_Be_Set()
        {
            // arrange
            var factory = new ConfigFactory(new ConfigReaders.ConfigReader(new MockValueProvider()));

            // act
            var sut = factory.Resolve<MyConfigPoco>();

            // assert
            Assert.Equal(-123.45M, sut.MyDecimal);
        }

        [Fact]
        public void Date_Can_Be_Set()
        {
            // arrange
            var factory = new ConfigFactory(new ConfigReaders.ConfigReader(new MockValueProvider()));

            // act
            var sut = factory.Resolve<MyConfigPoco>();

            // assert
            Assert.Equal(new DateTime(2015, 10, 21), sut.MyDate);
        }

        [Fact]
        public void Cannot_Set_Private_Member()
        {
            // arrange
            var factory = new ConfigFactory(new ConfigReaders.ConfigReader(new MockValueProvider()));

            // act
            var sut = factory.Resolve<MyConfigPoco>();

            // assert
            Assert.Equal(sut.MySetterIsPrivate, null);
        }

        [Fact]
        public void Can_Set_Enum()
        {
            // arrange
            var factory = new ConfigFactory(new ConfigReaders.ConfigReader(new MockValueProvider()));

            // act
            var sut = factory.Resolve<MyConfigPoco>();

            // assert
            Assert.Equal(MyEnum.Value2, sut.MyEnum);
        }

        [Fact]
        public void TypeConverter_IsRespected_On_Value()
        {
            // arrange
            var factory = new ConfigFactory(new ConfigReaders.ConfigReader(new MockValueProvider()));

            // act
            var sut = factory.Resolve<MyConfigPoco>();

            // assert
            Assert.Equal("prop1 configured", sut.MySubConfigPoco.Prop1);
            Assert.Equal("prop2 configured", sut.MySubConfigPoco.Prop2);
        }
    }
}




