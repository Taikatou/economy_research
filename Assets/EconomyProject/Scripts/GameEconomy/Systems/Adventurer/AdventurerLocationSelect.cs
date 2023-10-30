using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public class AdventurerLocationSelect : LocationSelect<Agent>
    {
        private static readonly EBattleEnvironments [] ValuesAsArray
            = Enum.GetValues(typeof(EBattleEnvironments)).Cast<EBattleEnvironments>().ToArray();
        
        public override int GetLimit(Agent agent)
        {
            return ValuesAsArray.Length;
        }

        public EBattleEnvironments GetBattle(Agent agent)
        {
            var location = GetCurrentLocation(agent);
            return ValuesAsArray[location];
        }

        private static readonly bool ObserveAdventurerLocation = true;
    
        public ObsData[] GetTravelObservations(Agent agent, AdventurerSystem system)
        {
            var currentParties = system.battleSubSystem.CurrentParties;
            var battle = ObserveAdventurerLocation ? GetBattle(agent) : EBattleEnvironments.Forest;
            
            var obs = new List<ObsData> {
                new CategoricalObsData<EBattleEnvironments>(battle) { Name="travelLocation" }
            };
            foreach (EBattleEnvironments b in Enum.GetValues(typeof(EBattleEnvironments)))
            {
                obs.AddRange(currentParties[b].GetObservations());
            }
            return obs.ToArray();
        }
        
        public ObsData[] GetTravelObservations(Agent agent)
        {
            var battle = ObserveAdventurerLocation ? GetBattle(agent) : EBattleEnvironments.Forest;
            
            var obs = new List<ObsData> {
                new CategoricalObsData<EBattleEnvironments>(battle) { Name="travelLocation" }
            };
            return obs.ToArray();
        }

        public static int SensorCount => SensorUtils<EBattleEnvironments>.Length + 
                                         (SensorUtils<EAdventurerTypes>.Length + 1) * (SystemTraining.PartySize - 1);
    }
}
