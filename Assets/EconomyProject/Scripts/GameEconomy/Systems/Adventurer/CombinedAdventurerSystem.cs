using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.GameEconomy.DataLoggers;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    [Serializable]
    public class CombinedAdventurerSystem
    {
        public BattleSubSystem battleSubSystem;
        
        public AdventurerLocationSelect locationSelect;
        
        public TravelSubSystem travelSubsystem;

        public BattleEnvironmentDataLogger dataLogger;
        
        public CombinedAdventurerSystem()
        {
            battleSubSystem = new BattleSubSystem(travelSubsystem, dataLogger);
        }

        public static int ObservationSize => 20;
        
        public void Start()
        {

        }
        
        public ObsData[] GetObservations(BaseAdventurerAgent agent, BufferSensorComponent[] bufferSensorComponent)
        {
            var obsData = locationSelect.GetTravelObservations(agent);
            return obsData;
        }

        public void SelectBattle<T>(T agent) where T : BaseAdventurerAgent
        {
            var location = locationSelect.GetBattle(agent);
            var getLootBox = travelSubsystem.GetLootBox(location);
            if (getLootBox.HasValue)
            {
                var adventurerInventory = agent.GetComponent<AdventurerRequestTaker>();
                adventurerInventory.CheckItemAdd(agent, getLootBox.Value.Resource, getLootBox.Value.Count, battleSubSystem.OnItemAdd, battleSubSystem.OnRequestComplete);  
            }
        }

        public void UpDownBattle(Agent agent, int movement)
        {
            locationSelect.MovePosition(agent, movement);
        }
        
        public EnabledInput[] GetEnabledInputs(Agent agent)
        {
            var inputChoices = new List<EAdventurerAgentChoices>
            {
                EAdventurerAgentChoices.Up,
                EAdventurerAgentChoices.Down,
                EAdventurerAgentChoices.Select
            };

            if (SystemTraining.IncludeShop)
            {
                inputChoices.Add(EAdventurerAgentChoices.Back);
            }

            var outputs = EconomySystemUtils<EAdventurerAgentChoices>.GetInputOfType(inputChoices);

            return outputs;
        }
        
        public void Update()
        {
            battleSubSystem.Update();
        }
    }
}
