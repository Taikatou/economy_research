using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public class AdventurerLocationSelect : LocationSelect<AdventurerAgent>
    {
        private static readonly EBattleEnvironments [] ValuesAsArray
            = Enum.GetValues(typeof(EBattleEnvironments)).Cast<EBattleEnvironments>().ToArray();
        
        public override int GetLimit(AdventurerAgent agent)
        {
            return ValuesAsArray.Length;
        }

        public EBattleEnvironments GetBattle(AdventurerAgent agent)
        {
            var location = GetCurrentLocation(agent);
            return ValuesAsArray[location];
        }
    
        public ObsData[] GetTravelObservations(AdventurerAgent agent, AdventurerSystem system)
        {
            var currentParties = system.battleSubSystem.CurrentParties; 
            var obs = new List<ObsData> {
                new CategoricalObsData<EBattleEnvironments>(GetBattle(agent)) { Name="travelLocation" }
            };
            foreach (EBattleEnvironments b in Enum.GetValues(typeof(EBattleEnvironments)))
            {
                obs.AddRange(currentParties[b].GetObservations());
            }
            return obs.ToArray();
        }

        public static int SensorCount => SensorUtils<EBattleEnvironments>.Length + 
                                         (SensorUtils<EAdventurerTypes>.Length + 1) * (SystemTraining.PartySize - 1);
    }
}
