using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{

    public delegate void SetChoice(BaseAdventurerAgent agent);
    
    [Serializable]
    public class AdventurerShopSystem : StateEconomySystem<BaseAdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
    {
        public AdventurerShopSubSystem adventurerShopSubSystem;
        public static int ObservationSize => 2;
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Shop;

        public override bool CanMove(BaseAdventurerAgent agent)
        {
            return true;
        }

        public override ObsData[] GetObservations(BaseAdventurerAgent agent, BufferSensorComponent[] bufferSensorComponent)
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

            adventurerShopSubSystem.GetObservations(bufferSensorComponent[0]);
            
            return obs.ToArray();
        }

        protected override void SetChoice(BaseAdventurerAgent agent, EAdventurerAgentChoices input)
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
                    BaseAdventurerAgent baseAdventurerAgent = null; //(AdventurerAgent) agent;
                    if (baseAdventurerAgent)
                    {
                        AgentInput.ChangeScreen(baseAdventurerAgent, EAdventurerScreen.Main);
                    }
                    break;
                case EAdventurerAgentChoices.Select:
                    adventurerShopSubSystem.PurchaseItem(agent);
                    break;
            }
        }

        public override EnabledInput[] GetEnabledInputs(Agent agent)
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
