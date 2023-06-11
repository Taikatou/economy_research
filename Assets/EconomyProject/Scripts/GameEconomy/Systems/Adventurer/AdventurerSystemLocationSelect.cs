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
        public Dictionary<EAdventurerSystem, EAdventurerScreen> GetMap;

        public override void Setup()
        {
            base.Setup();
                
            GetMap = new()
            {
                {EAdventurerSystem.Adventure, EAdventurerScreen.Adventurer},
                {EAdventurerSystem.Shop, EAdventurerScreen.Shop},
                
            };

            if (!TrainingConfig.AdventurerNoRequestMenu)
            {
                GetMap.Add(EAdventurerSystem.Request, EAdventurerScreen.Request);
            }
        }

        private EAdventurerSystem[] ValuesAsArray
        {
            get
            {
                return TrainingConfig.AdventurerNoRequestMenu
                    ? new[] { EAdventurerSystem.Adventure, EAdventurerSystem.Shop }
                    : new[] { EAdventurerSystem.Adventure, EAdventurerSystem.Shop, EAdventurerSystem.Request };
            }
        }
        
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
