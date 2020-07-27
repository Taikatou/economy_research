using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public class MainMenuSystem : EconomySystem<AdventurerAgent, AgentScreen>
    { 
        protected override AgentScreen ActionChoice => AgentScreen.Main;
        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }

        private void Update()
        {
            RequestDecisions();
        }
    }
}
