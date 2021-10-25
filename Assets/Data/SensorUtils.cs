using System;
using System.Linq;

namespace Data
{
    public static class SensorUtils<T> where T : Enum
    {
        public static int Length => Enum.GetValues(typeof(T)).Length;
        
        public static T [] ValuesToArray => Enum.GetValues(typeof(T)).Cast<T>().ToArray();
    }
}
