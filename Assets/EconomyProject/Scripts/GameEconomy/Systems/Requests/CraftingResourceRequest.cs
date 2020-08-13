using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class CraftingResourceRequest
    {
        private bool _resourceAdded;
        public CraftingResources Resource { get; }
        public CraftingInventory Inventory { get; }
        public int Number { get; set; }
        public int Price { get; set; }
        public int Reward => GetReward(Price);

        public int GetReward(int price)
        {
            return Number * Price;
        }

        private static readonly Dictionary<CraftingResources, int> _defaultPrice = new Dictionary<CraftingResources, int> {
            {CraftingResources.Wood, 5},
            {CraftingResources.Metal, 6},
            {CraftingResources.Gem, 7},
            {CraftingResources.DragonScale, 8}
        };

        public CraftingResourceRequest(CraftingResources resource, CraftingInventory inventory)
        {
            Number = 1;
            Resource = resource;
            Inventory = inventory;
            Price = _defaultPrice[resource];
        }

        public void TransferResource()
        {
            if (!_resourceAdded)
            {
                _resourceAdded = true;
                Inventory.AddResource(Resource, Number);
            }
        }
    }
}
