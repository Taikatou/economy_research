using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public enum EConfirmBattle { Confirm, Back }
    public class ConfirmBattleLocationSelect : LocationSelect<AdventurerAgent>
    {
        public override int GetLimit(AdventurerAgent agent)
        {
            return SensorUtils<EConfirmBattle>.Length;
        }
        
        public EConfirmBattle GetConfirmation(AdventurerAgent agent)
        {
            var location = GetCurrentLocation(agent);
            return SensorUtils<EConfirmBattle>.ValuesToArray[location];
        }
    }
}
