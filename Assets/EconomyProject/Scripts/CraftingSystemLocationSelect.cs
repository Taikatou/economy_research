using System;
using System.Linq;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts
{
    public class CraftingSystemLocationSelect : LocationSelect<ShopAgent>
    {
        protected override bool CircleOption => true;
        private static readonly ECraftingOptions [] ValuesAsArray
            = Enum.GetValues(typeof(ECraftingOptions)).Cast<ECraftingOptions>().ToArray();
        public override int GetLimit(ShopAgent agent)
        {
            return ValuesAsArray.Length;
        }

        public ECraftingOptions GetCraftingOption(ShopAgent agent)
        {
            return ValuesAsArray[GetCurrentLocation(agent)];
        }
    }
}
