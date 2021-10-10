using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors
{
    public class RequestTakerSensor : BaseEconomySensor
    {
        private readonly AdventurerRequestTaker _requestTaker;

        private readonly int _requestLimit;
        protected override float[] Data { get; }

        public override string GetName() => "RequestTakerSensor";

        public RequestTakerSensor(AdventurerRequestTaker requestTaker, int requestLimit)
        {
            _requestTaker = requestTaker;
            _requestLimit = requestLimit;

            var dataSize = (3 * _requestLimit * 2);
            Data = new float[dataSize];
            MObservationSpec = ObservationSpec.Vector(dataSize);
        }
        
        public override void Update()
        {
            // Player Observations
            var requests = _requestTaker.requestSystem.craftingRequestRecord.GetCurrentRequests(_requestTaker, _requestLimit);
            var i = 0;
            foreach (var t in requests)
            {
                var request = t ?? new CraftingResourceRequest(ECraftingResources.Nothing, null, 0, null);
                Data[i++] = (int)request .Resource;
                Data[i++] = request.Number;
                
                var amount = _requestTaker.GetCurrentStock(request.Resource);
                Data[i++] = amount;
            }

            foreach (var sense in _requestTaker.GetCurrentRequestData(_requestLimit))
            {
                Data[i++] = sense;
            }
        }
    }
}
