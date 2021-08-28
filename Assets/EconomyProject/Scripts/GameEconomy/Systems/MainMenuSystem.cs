using System;
using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    [Serializable]
    public class MainMenuSystem : EconomySystem<AdventurerAgent, EAdventurerScreen>
    {
        public override int ObservationSize => 0;
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Main;
        
        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }

        public override float [] GetObservations(AdventurerAgent agent)
        {
            return Array.Empty<float>();
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
