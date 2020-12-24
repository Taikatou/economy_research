using System;
using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    [Serializable]
    public class MainMenuSystem : EconomySystem<AdventurerAgent, EAdventurerScreen>
    { 
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Main;
        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }

        public override float [] GetSenses(AdventurerAgent agent)
        {
            return new float [0];
        }

        public override InputAction[] GetInputOptions(AdventurerAgent agent)
        {
            var agentScreen = EconomySystemUtils.GetStateInput<EAdventurerScreen>();
            return agentScreen.ToArray();
        }

        public override void SetChoice(AdventurerAgent agent, int input)
        {
            if (Enum.IsDefined(typeof(EAdventurerScreen), input))
            {
                AgentInput.ChangeScreen(agent, (EAdventurerScreen) input);
            }
        }

        private void Update()
        {
            RequestDecisions();
        }
    }
}
