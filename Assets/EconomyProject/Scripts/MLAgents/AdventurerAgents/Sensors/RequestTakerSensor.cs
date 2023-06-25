using System;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors
{
    public class RequestTakerSensor : BaseEconomySensor
    {
        private static readonly int CraftingResourceList = Enum.GetNames(typeof(ECraftingResources)).Length - 1;
        
        private readonly BufferSensorComponent _requestTakenBufferComponent;
        
        private readonly BufferSensorComponent _requestAvailableBufferComponent;
        
        private readonly AdventurerRequestTaker _requestTaker;
        
        protected override float[] Data { get; }

        public override string GetName() => "RequestTakerSensor";

        private int SensorCount => 0;

        public RequestTakerSensor(BufferSensorComponent requestBufferComponent, 
            BufferSensorComponent requestTakenBufferComponent, AdventurerRequestTaker requestTaker) : base(null)
        {
            _requestTaker = requestTaker;
            _requestAvailableBufferComponent = requestBufferComponent;
            _requestTakenBufferComponent = requestTakenBufferComponent;

            Data = new float[SensorCount];
            MObservationSpec = ObservationSpec.Vector(SensorCount);
        }
        
        public override void Update()
        {
            // Player Observations
            var requests = _requestTaker.requestSystem.craftingRequestRecord.GetCurrentRequests(_requestTaker);
            foreach (var request in requests)
            {
                var obs = new float[CraftingResourceList + 2];
                obs[(int)request.Resource - 1] = 1;
                obs[CraftingResourceList] = request.Number;

                var amount = _requestTaker.GetCurrentStock(request.Resource);
                obs[CraftingResourceList + 1] = amount;
                
                _requestTakenBufferComponent.AppendObservation(obs);
            }

            _requestTaker.GetCurrentRequestData(_requestAvailableBufferComponent);
        }
    }
}
