using System;
using System.ComponentModel;
using System.Globalization;

namespace ConfigFactory
{
    public class MagicalConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var tmp = value as string;

            if (tmp == null) return base.ConvertFrom(context, culture, value);

            if (tmp == "magic") return new DateTime(2015, 10, 12);

            return base.ConvertFrom(context, culture, value);
        }
    }
}