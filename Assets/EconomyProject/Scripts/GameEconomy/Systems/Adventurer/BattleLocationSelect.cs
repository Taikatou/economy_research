using System;
using System.Linq;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public class BattleLocationSelect : LocationSelect<AdventurerAgent>
    {
        private static readonly EBattleAction [] valuesAsArray
            = Enum.GetValues(typeof(EBattleAction)).Cast<EBattleAction>().ToArray();
        public override int GetLimit(AdventurerAgent agent)
        {
            return valuesAsArray.Length;
        }

        public EBattleAction GetBattleAction(AdventurerAgent agent)
        {
            var location = GetCurrentLocation(agent);
            return valuesAsArray[location];
        }
    }
}
