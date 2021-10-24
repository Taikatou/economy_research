using System;
using System.Linq;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public class AdventurerLocationSelect : LocationSelect<AdventurerAgent>
    {
        private static readonly EBattleEnvironments [] valuesAsArray
            = Enum.GetValues(typeof(EBattleEnvironments)).Cast<EBattleEnvironments>().ToArray();
        
        public override int GetLimit(AdventurerAgent agent)
        {
            return valuesAsArray.Length;
        }

        public EBattleEnvironments GetBattle(AdventurerAgent agent)
        {
            var location = GetCurrentLocation(agent);
            return valuesAsArray[location];
        }
    
        public ObsData[] GetTravelObservations(AdventurerAgent agent)
        {
            return new [] { new CategoricalObsData<EBattleEnvironments>(GetBattle(agent))
            {
                Name="travelLocation",
            }};
        }

        public static int SensorCount => 1;
    }
}
