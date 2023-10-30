using System;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    [Serializable]
    public class CombinedAdventurerSystem
    {
        public BattleSubSystem<NewAdventurerAgent> battleSubSystem;
        
        public AdventurerLocationSelect locationSelect;
        
        public TravelSubSystem travelSubsystem;

        public static int ObservationSize => 20;
        
        public void Start()
        {

        }
        
        public ObsData[] GetObservations(AdventurerAgent agent, BufferSensorComponent[] bufferSensorComponent)
        {
            var obsData = locationSelect.GetTravelObservations(agent);
            return obsData;
        }

        public void Select(NewAdventurerAgent agent)
        {
            var location = locationSelect.GetBattle(agent);
            var getLootBox = travelSubsystem.GetLootBox(location);
            if (getLootBox.HasValue)
            {
                var adventurerInventory = agent.GetComponent<AdventurerRequestTaker>();
                adventurerInventory.CheckItemAdd(agent, getLootBox.Value.Resource, getLootBox.Value.Count, battleSubSystem.OnItemAdd, battleSubSystem.OnRequestComplete);  
            }
        }
    }
}
