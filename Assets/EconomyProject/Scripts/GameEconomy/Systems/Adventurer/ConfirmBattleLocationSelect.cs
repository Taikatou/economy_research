using System.Collections.Generic;
using Data;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.UI;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public enum EConfirmBattle { Confirm, Back }
    public class ConfirmBattleLocationSelect : LocationSelect<AdventurerAgent>
    { 
        public AdventurerSystemBehaviour adventurerSystem;
        public static int SensorCount => SensorUtils<EConfirmBattle>.Length + 1 + ((SensorUtils<EAdventurerTypes>.Length + 1) * SystemTraining.PartySize);
        public override int GetLimit(AdventurerAgent agent)
        {
            return SensorUtils<EConfirmBattle>.Length;
        }
        
        public EConfirmBattle GetConfirmation(AdventurerAgent agent)
        {
            var location = GetCurrentLocation(agent);
            return SensorUtils<EConfirmBattle>.ValuesToArray[location];
        }

        public ObsData[] GetConfirmationObservations(AdventurerAgent agent, AdventurerSystem system)
        {
            var currentParties = adventurerSystem.system.battleSubSystem.GetSubsystem(agent);

            var environment = adventurerSystem.system.battleSubSystem.GetBattleEnvironment(agent);
            var obs = new List<ObsData> {
                new CategoricalObsData<EConfirmBattle>(GetConfirmation(agent)) { Name="confirmation" },
                new SingleObsData() {data=environment.HasValue? 1 : 0},
                new CategoricalObsData<EBattleEnvironments>(environment.HasValue?environment.Value : EBattleEnvironments.Forest)
            };

            obs.AddRange(currentParties.GetObservations());

            return obs.ToArray();
        }
    }
}
