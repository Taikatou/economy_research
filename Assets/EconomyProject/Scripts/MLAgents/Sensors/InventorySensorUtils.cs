using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;

namespace EconomyProject.Scripts.MLAgents.Sensors
{
    public static class InventorySensorUtils
    {
        private static readonly ECraftingResources [] CraftingAsList = Enum.GetValues(typeof(ECraftingResources)).Cast<ECraftingResources>().ToArray();
        public static List<ObsData> GetInventoryData(AgentInventory agentInventory)
        {
            var toReturn = new List<ObsData>();
            return toReturn;
        }
    }
}
