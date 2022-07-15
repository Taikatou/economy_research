using System.Linq;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public class BattleLocationSelect : LocationSelect<AdventurerAgent>
    {
        public AdventurerSystemBehaviour aSystem;

        private EBattleAction[] GetBattleActions(AdventurerAgent agent)
        {
            var subSystem = aSystem.system.battleSubSystem.GetSubSystem(agent);
            if (subSystem != null)
            {
                var playerData = subSystem.PlayerFighterUnits.GetAgentPlayerData(agent.GetHashCode());
                var map = playerData.AttackActionMap;
                return map.Keys.ToArray();
            }
            return new EBattleAction [] { };
        }
        public override int GetLimit(AdventurerAgent agent)
        {
            return GetBattleActions(agent).Length;
        }

        public EBattleAction GetBattleAction(AdventurerAgent agent)
        {
            var valuesAsArray = GetBattleActions(agent);
            var location = GetCurrentLocation(agent);
            if (location >= valuesAsArray.Length)
            {
                return EBattleAction.PrimaryAction;
            } 
            return valuesAsArray.ElementAtOrDefault(location);
        }
    }
}
