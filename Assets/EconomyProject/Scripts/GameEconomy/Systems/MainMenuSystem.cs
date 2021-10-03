using System;
using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    [Serializable]
    public class MainMenuSystem : EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
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

        protected override void SetChoice(AdventurerAgent agent, EAdventurerAgentChoices input)
        {
            var s = (EAdventurerScreen) input;
            if (Enum.IsDefined(typeof(EAdventurerScreen), s))
            {
                Debug.Log(input);
                AgentInput.ChangeScreen(agent, s);
            }
        }

        public override EnabledInput[] GetEnabledInputs(AdventurerAgent agent)
        {
            var inputChoices = new[]
            {
                EAdventurerAgentChoices.Shop,
                EAdventurerAgentChoices.FindRequest,
                EAdventurerAgentChoices.Adventure
            };
            var outputs = EconomySystemUtils<EAdventurerAgentChoices>.GetInputOfType(inputChoices);

            return outputs;
        }
    }
}
