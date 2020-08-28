using System;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public enum AdventurerRequestInput {Back}
    public class RequestAdventurerSystem : EconomySystem<AdventurerAgent, EAdventurerScreen>
    {
        public RequestSystem requestSystem;
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Request;
        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }

        public override float[] GetSenses(AdventurerAgent agent)
        {
            var input = new float[CraftingResourceRequest.SensorCount + CraftingResourceRequest.SensorCount];

            var itemList = agent.requestTaker.GetItemList();
            var senseA = CraftingResourceRequest.GetSenses(itemList, 5);
            senseA.CopyTo(input, 0);
            var senseB = requestSystem.GetSenses(agent);
            senseB.CopyTo(input, senseA.Length);
            return requestSystem.GetSenses(agent);
        }

        public override InputAction[] GetInputOptions(AdventurerAgent agent)
        {
            return EconomySystemUtils.GetStateInput<AdventurerRequestInput>().ToArray();
        }

        private void Update()
        {
            RequestDecisions();
        }

        public override void SetChoice(AdventurerAgent agent, int input)
        {
            if (Enum.IsDefined(typeof(AdventurerRequestInput), input))
            {
                AgentInput.ChangeScreen(agent, EAdventurerScreen.Main);
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
