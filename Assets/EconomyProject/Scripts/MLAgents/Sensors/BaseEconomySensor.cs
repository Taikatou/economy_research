using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.Sensors
{
    public abstract class BaseEconomySensor : ISensor
    {
        protected ObservationSpec MObservationSpec { get; set; }
        protected abstract float [] Data { get; }

        public ObservationSpec GetObservationSpec()
        {
            return MObservationSpec;
        }

        public int Write(ObservationWriter writer)
        {
//            Debug.Log(GetName() + "\t" + Data.Length + "\t" + MObservationSpec.Shape);
            writer.AddList(Data);
            return Data.Length;
        }

        public byte[] GetCompressedObservation()
        {
            return null;
        }

        public abstract void Update();

        public void Reset()
        {
            for(var i = 0; i < Data.Length; i++)
            {
                Data[i] = 0;
            }
        }

        public CompressionSpec GetCompressionSpec()
        {
            return CompressionSpec.Default();
        }

        public abstract string GetName();
    }
}
