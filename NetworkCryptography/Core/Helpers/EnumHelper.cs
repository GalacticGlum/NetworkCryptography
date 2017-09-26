using System;

namespace NetworkCryptography.Core.Helpers
{
    public static class EnumHelper
    {
        public static T GetValue<T>(int index)
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum) throw new ArgumentException($"{enumType.FullName} is not an enum!");

            T[] values = (T[])Enum.GetValues(typeof(T));
            return values[index];
        }
    }
}
