using System;
using System.Linq;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts
{
    public class RequestSystemLocationSelect : LocationSelect<ShopAgent>
    {
        protected override bool CircleOption => true;
        private static readonly EShopRequestStates [] ValuesAsArray
            = Enum.GetValues(typeof(EShopRequestStates)).Cast<EShopRequestStates>().ToArray();
        public override int GetLimit(ShopAgent agent)
        {
            return ValuesAsArray.Length;
        }

        public EShopRequestStates GetShopOption(ShopAgent agent)
        {
            return ValuesAsArray[GetCurrentLocation(agent)];
        }
    }
}
