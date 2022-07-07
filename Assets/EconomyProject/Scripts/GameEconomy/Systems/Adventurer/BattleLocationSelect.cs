using System;
using System.Linq;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public class BattleLocationSelect : LocationSelect<AdventurerAgent>
    {
        public AdventurerSystemBehaviour aSystem;
        private static readonly EBattleAction [] ValuesAsArray
            = Enum.GetValues(typeof(EBattleAction)).Cast<EBattleAction>().ToArray();
        public override int GetLimit(AdventurerAgent agent)
        {
            var subSystem = aSystem.system.battleSubSystem.GetSubSystem(agent);
            if (subSystem != null)
            {
                var playerData = subSystem.PlayerFighterUnits.GetAgentPlayerData(agent.GetHashCode());
                var map = playerData.AttackActionMap;

                return map.Count;
            }

            return 0;
        }

        public EBattleAction GetBattleAction(AdventurerAgent agent)
        {
            var location = GetCurrentLocation(agent);
            return ValuesAsArray[location];
        }
    }
}
