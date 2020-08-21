using System;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public enum AdventurerRequestInput {Back}
    public class RequestAdventurerSystem : EconomySystem<AdventurerAgent, AgentScreen>
    {
        public RequestSystem requestSystem;
        protected override AgentScreen ActionChoice => AgentScreen.Request;
        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }

        public override float[] GetSenses(AdventurerAgent agent)
        {
            var input = new float[CraftingResourceRequest.SensorCount + CraftingResourceRequest.SensorCount];
            
            var senseA = CraftingResourceRequest.GetSenses(agent.requestTaker.ItemList, 5);
            senseA.CopyTo(input, 0);
            var senseB = requestSystem.GetSenses(agent);
            senseB.CopyTo(input, senseA.Length);
            return requestSystem.GetSenses(agent);
        }

        private void Update()
        {
            RequestDecisions();
        }

        public override void SetChoice(AdventurerAgent agent, int input)
        {
            if (Enum.IsDefined(typeof(AdventurerRequestInput), input))
            {
                AgentInput.ChangeScreen(agent, AgentScreen.Main);
            }
            else
            {
                input -= 1;
                var requests = requestSystem.GetAllCraftingRequests();
                if (input >= 0 && input < requests.Count)
                {
                    var requestTaker = agent.GetComponent<AdventurerRequestTaker>();
                    requestTaker.TakeRequest(requests[input]);
                }   
            }
        }
    }
}
