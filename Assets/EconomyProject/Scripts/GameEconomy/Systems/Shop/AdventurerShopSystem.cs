using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{

    public delegate void SetChoice(AdventurerAgent agent);
    
    [Serializable]
    public class AdventurerShopSystem : StateEconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
    {
        public AdventurerShopSubSystem adventurerShopSubSystem;
        public static int ObservationSize => 2;
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Shop;

        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }

        public override ObsData[] GetObservations(AdventurerAgent agent, BufferSensorComponent bufferSensorComponent)
        {
            var obs = new List<ObsData>
            {
                new SingleObsData
                {
                    data=adventurerShopSubSystem.GetCurrentLocation(agent),
                    Name="scrollLocation",
                },
                new SingleObsData
                {
                    data=adventurerShopSubSystem.GetLimit(agent),
                    Name="scrollLocation",
                }
            };

            adventurerShopSubSystem.GetObservations(bufferSensorComponent);
            
            return obs.ToArray();
        }

        protected override void SetChoice(AdventurerAgent agent, EAdventurerAgentChoices input)
        {
            switch (input)
            {
                case EAdventurerAgentChoices.Down:
                    adventurerShopSubSystem.MovePosition(agent, -1);
                    break;
                
                case EAdventurerAgentChoices.Up:
                    adventurerShopSubSystem.MovePosition(agent, 1);
                    break;
                
                case EAdventurerAgentChoices.Back:
                    AgentInput.ChangeScreen(agent, EAdventurerScreen.Main);
                    break;
                case EAdventurerAgentChoices.Select:
                    adventurerShopSubSystem.PurchaseItem(agent);
                    break;
            }
        }

        public override EnabledInput[] GetEnabledInputs(AdventurerAgent agent)
        {
            var inputChoices = new List<EAdventurerAgentChoices>
            {
                EAdventurerAgentChoices.Down,
                EAdventurerAgentChoices.Up,
                EAdventurerAgentChoices.Back,
                EAdventurerAgentChoices.Select
            };

            var outputs = EconomySystemUtils<EAdventurerAgentChoices>.GetInputOfType(inputChoices);

            return outputs;
        }
    }
}
