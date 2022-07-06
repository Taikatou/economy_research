using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts.AI;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public class ConfirmAbilitiesLocationSelect : LocationSelect<AdventurerAgent>
    {
        public static int SensorCount = SensorUtils<EAttackOptions>.Length;
        public override int GetLimit(AdventurerAgent agent)
        {
            return PlayerActionMap.GetAbilities(agent.AdventurerType, agent.levelComponent.Level).Count;
        }

        public EAttackOptions GetAbility(AdventurerAgent agent)
        {
            if (agent != null)
            {
                var abilities = PlayerActionMap.GetAbilities(agent.AdventurerType, agent.levelComponent.Level);
                return abilities[GetCurrentLocation(agent)];
            }

            return EAttackOptions.None;
        }

        public ObsData[] GetObservations(AdventurerAgent agent)
        {
            var ability = GetAbility(agent);
            Debug.Log(ability);
            return new[] {new CategoricalObsData<EAttackOptions>(ability)};
        }
    }
}
