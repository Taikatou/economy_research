using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public enum EAdventurerSystem { Adventure, Shop, Request}
    public class AdventurerSystemLocationSelect : LocationSelect<BaseAdventurerAgent>
    {
        public Dictionary<EAdventurerSystem, EAdventurerScreen> GetMap;
        public bool Initialized = false;
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

            Initialized = true;
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
        
        public override int GetLimit(BaseAdventurerAgent agent)
        {
            return ValuesAsArray.Length;
        }

        public EAdventurerSystem GetEnvironment(BaseAdventurerAgent agent)
        {
            var location = GetCurrentLocation(agent);
            return ValuesAsArray[location];
        }

        public static int SensorCount => SensorUtils<EAdventurerSystem>.Length;

        public ObsData[] GetTravelObservations(BaseAdventurerAgent agent)
        {
            var obs = new List<ObsData> {
                new CategoricalObsData<EAdventurerSystem>(GetEnvironment(agent)) { Name="travelLocation" }
            };
            return obs.ToArray();
        }
    }
}
