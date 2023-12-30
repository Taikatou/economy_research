using System;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors
{
    public class RequestTakerSensor : BaseEconomySensor
    {
        private readonly BufferSensorComponent _requestAvailableBufferComponent;
        
        private readonly AdventurerRequestTaker _requestTaker;
        
        protected override float[] Data => _data;

        private float[] _data;

        public override string GetName() => "RequestTakerSensor";

        private int SensorCount => 4;

        public RequestTakerSensor(AdventurerRequestTaker requestTaker) : base(null)
        {
            _requestTaker = requestTaker;

            _data = new float[SensorCount];
            MObservationSpec = ObservationSpec.Vector(SensorCount);
        }
        
        public override void Update()
        {
            _data = _requestTaker.GetCurrentRequestData();
        }
    }
}
