using System;
using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

public class AdventurerLocationSelect : LocationSelect<AdventurerAgent>
{
    private static readonly EBattleEnvironments [] valuesAsArray
        = Enum.GetValues(typeof(EBattleEnvironments)).Cast<EBattleEnvironments>().ToArray();
    protected override int GetLimit(AdventurerAgent agent)
    {
        return valuesAsArray.Length;
    }

    public EBattleEnvironments GetBattle(AdventurerAgent agent)
    {
        var location = GetCurrentLocation(agent);
        return valuesAsArray[location];
    }
}
