using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class RequestAdventurerSystem : EconomySystem<AdventurerAgent, AgentScreen>
    {
        protected override AgentScreen ActionChoice => AgentScreen.Request;
        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }
        
        private void Update()
        {
            RequestDecisions();
        }

        public void MakeChoice(AdventurerAgent agent, ResourceRequest request)
        {
            var requestTaker = agent.GetComponent<RequestTaker>();
            requestTaker.TakeRequest(request);
        }
    }
}
