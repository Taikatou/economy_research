using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.Adventurer;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    [Serializable]
    public class MainMenuSystem : EconomySystem<BaseAdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
    {
        public AdventurerSystemLocationSelect adventurerSystemLocationSelect;
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Main;
        
        public override bool CanMove(BaseAdventurerAgent agent)
        {
            return true;
        }

        public override ObsData [] GetObservations(BaseAdventurerAgent agent, BufferSensorComponent[] bufferSensorComponent)
        {
            return adventurerSystemLocationSelect.GetTravelObservations(agent);
        }

        protected override void SetChoice(BaseAdventurerAgent agent, EAdventurerAgentChoices input)
        {
            switch (input)
            {
                case EAdventurerAgentChoices.Up:
                    UpDown(agent, 1);
                    break;
                case EAdventurerAgentChoices.Down:
                    UpDown(agent, -1);
                    break;
                case EAdventurerAgentChoices.Select:
                    ChooseScreen(agent);
                    break;
            }
        }
        
        public void UpDown(BaseAdventurerAgent agent, int movement)
        {
            adventurerSystemLocationSelect.MovePosition(agent, movement);
        }

        public void ChooseScreen(BaseAdventurerAgent agent)
        {
            var system = adventurerSystemLocationSelect.GetEnvironment(agent);
            if (adventurerSystemLocationSelect.Initialized)
            {
                var map = adventurerSystemLocationSelect.GetMap[system];
                AgentInput.ChangeScreen(agent, map);   
            }
        }

        public override EnabledInput[] GetEnabledInputs(BaseAdventurerAgent agent, int branch)
        {
            var inputChoices = new[]
            {
                EAdventurerAgentChoices.Up,
                EAdventurerAgentChoices.Down,
                EAdventurerAgentChoices.Select
            };
            var outputs = EconomySystemUtils<EAdventurerAgentChoices>.GetInputOfType(inputChoices, branch);

            return outputs;
        }
    }
}
