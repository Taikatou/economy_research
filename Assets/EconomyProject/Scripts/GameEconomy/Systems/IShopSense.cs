﻿using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public interface IShopSense
    {
        float [] GetSenses(ShopAgent agent);
    }
}
