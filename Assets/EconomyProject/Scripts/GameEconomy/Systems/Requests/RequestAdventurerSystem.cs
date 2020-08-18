using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
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
            return new[] { 0.0f };
        }

        private void Update()
        {
            RequestDecisions();
        }

        public override void SetChoice(AdventurerAgent agent, int input)
        {
            var requests = requestSystem.GetAllCraftingRequests();
            if (input > requests.Count)
            {
                agent.GetComponent<AdventurerRequestTaker>().TakeRequest(requests[input]);
            }
        }
    }
}
