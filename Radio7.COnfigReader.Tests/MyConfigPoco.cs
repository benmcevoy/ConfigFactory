using System;

namespace Radio7.ConfigReader.Tests
{
    internal class MyConfigPoco
    {
        public string MyStringHasNoBackingField { get; set; }

        private string _myStringHasABackingField = "default";
        public string MyStringHasABackingField
        {
            get { return _myStringHasABackingField; }
            set { _myStringHasABackingField = value; }
        }

        public string MyFieldHasADefault = "default";

        public DateTime MyDate;
        public int MyInt = 456;
        public uint MyUInt = 456;
        public byte MyByte = 127;
        public sbyte MySByte = 127;
        public short MyShort = 456;
        public ushort MyUShort = 456;
        public long MyLong = 456;
        public ulong MyULong = 456;
        public float MyFloat = 456;
        public Single MySingle = 456;
        public double MyDouble = 456;
        public char MyChar;
        public bool MyBool;
        public Object MyObject;
        public string MyString;
        public decimal MyDecimal;

        public string MySetterIsPrivate { get; private set; }
        private string MyFieldIsPrivate;

        public MyEnum MyEnum;
    }

    internal enum MyEnum
    {
        Value1,
        Value2
    }
}