using System;
using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public enum AdventurerRequestInput {Back=0}
    [Serializable]
    public class RequestAdventurerSystem : EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
    {
        public RequestSystem requestSystem;
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Request;

        public override int ObservationSize =>
            CraftingResourceRequest.SensorCount + CraftingResourceRequest.SensorCount;
        
        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }
        public override float[] GetObservations(AdventurerAgent agent)
        {
            var input = new float[CraftingResourceRequest.SensorCount + CraftingResourceRequest.SensorCount];

            var itemList = agent.requestTaker.GetItemList();
            var senseA = CraftingResourceRequest.GetObservations(itemList);
            senseA.CopyTo(input, 0);
            var senseB = requestSystem.GetObservations(agent);
            senseB.CopyTo(input, senseA.Length);
            return input;
        }

        public void Update()
        {
            RequestDecisions();
        }

        public override void SetChoice(AdventurerAgent agent, EAdventurerAgentChoices input)
        {
            if (Enum.IsDefined(typeof(AdventurerRequestInput), input))
            {
                AgentInput.ChangeScreen(agent, EAdventurerScreen.Main);
            }
            else
            {
          /*      input -= 1;
                var requests = requestSystem.GetAllCraftingRequests();
                if (input >= 0 && input < requests.Count)
                {
                    var requestTaker = agent.GetComponent<AdventurerRequestTaker>();
                    requestTaker.TakeRequest(requests[input]);
                }   */
            }
        }

        private List<InputAction> GetRequestInput()
        {
            var output = new List<InputAction>();
            var requests = requestSystem.GetAllCraftingRequests();
            var i = 1;
            foreach (var req in requests)
            {
                output.Add(new InputAction{Action = req.ToString(), ActionNumber = i});
                i++;
            }

            return output;
        }
        
        public override EnabledInput[] GetEnabledInputs(AdventurerAgent agent)
        {
            var inputChoices = new[]
            {
                EAdventurerAgentChoices.TakeResourceRequest
            };
            var outputs = AdventurerEconomySystemUtils.GetInputOfType(inputChoices);

            return outputs;
        }
    }
}
