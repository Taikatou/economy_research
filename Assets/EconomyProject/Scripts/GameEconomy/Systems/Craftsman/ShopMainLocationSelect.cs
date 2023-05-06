using System;
using System.Linq;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.GameEconomy.Systems.Craftsman
{
    public class ShopMainLocationSelect : LocationSelect<ShopAgent>
    {
        private static readonly EShopScreen[] ValuesAsArray
            = { EShopScreen.Craft, EShopScreen.Request };
        public override int GetLimit(ShopAgent agent)
        {
            return ValuesAsArray.Length;
        }
        
        public EShopScreen GetMenu(ShopAgent agent)
        {
            var location = GetCurrentLocation(agent);
            return ValuesAsArray[location];
        }
    }
}
