using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts.AI;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public class ConfirmAbilitiesLocationSelect : LocationSelect<BaseAdventurerAgent>
    {
        public static readonly int SensorCount = SensorUtils<EAttackOptions>.Length + 1;
        public override int GetLimit(BaseAdventurerAgent agent)
        {
            return PlayerActionMap.GetAbilities(agent.AdventurerType, agent.LevelComponent.Level).Count;
        }

        public EAttackOptions GetAbility(BaseAdventurerAgent agent)
        {
            if (agent != null)
            {
                var abilities = PlayerActionMap.GetAbilities(agent.AdventurerType, agent.LevelComponent.Level);
                return abilities[GetCurrentLocation(agent)];
            }

            return EAttackOptions.None;
        }

        public ObsData[] GetObservations(BaseAdventurerAgent agent)
        {
            var selected = (float)GetCurrentLocation(agent) / (float) GetLimit(agent);
            var ability = GetAbility(agent);
            return new ObsData[]
            {
                new CategoricalObsData<EAttackOptions>(ability),
                new SingleObsData{data=selected}
            };
        }
    }
}
