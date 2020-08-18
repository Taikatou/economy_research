using System;
using System.Collections.Generic;
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

        public override float [] GetSenses(AdventurerAgent agent)
        {
            return new float [0];
        }

        public override void SetChoice(AdventurerAgent agent, int input)
        {
            if (Enum.IsDefined(typeof(AgentScreen), input))
            {
                AgentInput.ChangeScreen(agent, (AgentScreen) input);
            }
        }

        private void Update()
        {
            RequestDecisions();
        }
    }
}
