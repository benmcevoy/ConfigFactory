using System;
using System.ComponentModel;
using System.Globalization;

namespace Radio7.ConfigReader.Tests
{
    internal class MyValueConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var tmp = value as string;

            if (tmp == null) return base.ConvertFrom(context, culture, value);

            var props = tmp.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

            return new MySubConfigPoco
            {
                Prop1 = props[0],
                Prop2 = props[1]
            };
        }
    }
}
