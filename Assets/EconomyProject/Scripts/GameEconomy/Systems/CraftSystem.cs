using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public class CraftSystem : EconomySystem<ShopAgent, EShopScreen>
    {
        protected override EShopScreen ActionChoice => EShopScreen.Craft;
        public override bool CanMove(ShopAgent agent)
        {
            return true;
        }
    }
}
