using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    [Serializable]
    public struct AdventurerRestrictions
    {
        public EBattleEnvironments environment;

        public int requiredDamage;
    }
    
    public class AdventurerLocationSelect : LocationSelect<BaseAdventurerAgent>
    {
        private static readonly EBattleEnvironments [] ValuesAsArray
            = Enum.GetValues(typeof(EBattleEnvironments)).Cast<EBattleEnvironments>().ToArray();

        public AdventurerRestrictions[] adventurerRestrictions;

        private Dictionary<EBattleEnvironments, int> requiredDamageMap;

        public override void Setup()
        {
            base.Setup();
            requiredDamageMap = new Dictionary<EBattleEnvironments, int>();
            foreach (var item in adventurerRestrictions)
            {
                requiredDamageMap.Add(item.environment, item.requiredDamage);
            }
        }

        public override int GetLimit(BaseAdventurerAgent agent)
        {
            if (requiredDamageMap == null)
                return 0;
            var limit = 0;
            foreach (var item in ValuesAsArray)
            {
                if (TrainingConfig.RestrictMovement)
                {
                    if (agent.AdventurerInventory.EquipedItem.itemDetails.damage < requiredDamageMap[item])
                    {
                        break;
                    }
                }
                limit++;
            }
            return limit;
        }

        public EBattleEnvironments GetBattle(BaseAdventurerAgent agent)
        {
            var location = GetCurrentLocation(agent);
            return ValuesAsArray[location];
        }

        private static readonly bool ObserveAdventurerLocation = true;
    
        public ObsData[] GetTravelObservations(BaseAdventurerAgent agent, AdventurerSystem system)
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
        
        public ObsData[] GetTravelObservations(BaseAdventurerAgent agent)
        {
            return new ObsData[] { };
        }

        public static int SensorCount => SensorUtils<EBattleEnvironments>.Length + 
                                         (SensorUtils<EAdventurerTypes>.Length + 1) * (SystemTraining.PartySize - 1);
    }
}
