using System;

namespace Data
{
    public static class SensorUtils<T> where T : Enum
    {
        public static int Length => Enum.GetValues(typeof(T)).Length;
    }
}
