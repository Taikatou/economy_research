using System;
using System.Linq;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public class AdventurerLocationSelect : LocationSelect<AdventurerAgent>
    {
        private static readonly EBattleEnvironments [] valuesAsArray
            = Enum.GetValues(typeof(EBattleEnvironments)).Cast<EBattleEnvironments>().ToArray();
        protected override int GetLimit(AdventurerAgent agent)
        {
            return valuesAsArray.Length;
        }

        public EBattleEnvironments GetBattle(AdventurerAgent agent)
        {
            var location = GetCurrentLocation(agent);
            return valuesAsArray[location];
        }
    
        public float[] GetTravelObservations(AdventurerAgent agent)
        {
            return new float[] {GetCurrentLocation(agent)};
        }

        public int SensorCount = 1;
    }
}
