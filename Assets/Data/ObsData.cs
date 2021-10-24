using System;
using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public abstract class ObsData
    {
        public string Name;
        public abstract float [] GetData { get; }

        public static float[] GetEnumerableData(IList<ObsData> data)
        {
            var length = data.Sum(d => d.GetData.Length);
            var outputData = new float[length];
            var counter = 0;
            for (var i = 0; i < data.Count; i++)
            {
                var obs = data[i].GetData;
                foreach (var t in obs)
                {
                    outputData[i] = t;
                    counter++;
                }
            }

            return outputData;
        }
    };

    public class SingleObsData : ObsData
    {
        public float data;
        
        public override float [] GetData => new []{ data };
    }

    public class BaseCategoricalObsData : ObsData
    {
        private readonly float [] _oneHotEncoding;
        public BaseCategoricalObsData(int data, int categoricalRange)
        {
            _oneHotEncoding = new float[categoricalRange];
            for (var i = 0; i < categoricalRange; i++)
            {
                _oneHotEncoding[i] = i == data ? 1.0f : 0.0f;
            }
        }
        public override float[] GetData => _oneHotEncoding;
    }

    public class CategoricalObsData<T> : BaseCategoricalObsData where T : Enum
    {
        public CategoricalObsData(T data) : base (Convert.ToInt32(data), SensorUtils<T>.Length)
        {
        }
    }
}
