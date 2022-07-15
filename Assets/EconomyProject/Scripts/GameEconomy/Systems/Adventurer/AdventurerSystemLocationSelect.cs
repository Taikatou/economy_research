using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public enum EAdventurerSystem { Adventure, Shop, Request}
    public class AdventurerSystemLocationSelect : LocationSelect<AdventurerAgent>
    {
        public static readonly Dictionary<EAdventurerSystem, EAdventurerScreen> GetMap = new Dictionary<EAdventurerSystem, EAdventurerScreen>
        {
            {EAdventurerSystem.Adventure, EAdventurerScreen.Adventurer},
            {EAdventurerSystem.Shop, EAdventurerScreen.Shop},
            {EAdventurerSystem.Request, EAdventurerScreen.Request}
        };
        
        private static readonly EAdventurerSystem [] ValuesAsArray
            = Enum.GetValues(typeof(EAdventurerSystem)).Cast<EAdventurerSystem>().ToArray();
        
        public override int GetLimit(AdventurerAgent agent)
        {
            return ValuesAsArray.Length;
        }

        public EAdventurerSystem GetEnvironment(AdventurerAgent agent)
        {
            var location = GetCurrentLocation(agent);
            return ValuesAsArray[location];
        }

        public static int SensorCount => SensorUtils<EAdventurerSystem>.Length;

        public ObsData[] GetTravelObservations(AdventurerAgent agent)
        {
            var obs = new List<ObsData> {
                new CategoricalObsData<EAdventurerSystem>(GetEnvironment(agent)) { Name="travelLocation" }
            };
            return obs.ToArray();
        }
    }
}
