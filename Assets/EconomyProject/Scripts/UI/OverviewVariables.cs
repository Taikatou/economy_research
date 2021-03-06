﻿namespace EconomyProject.Scripts.UI
{
    public static class LearningStats
    {
        public static float AdventurerLossPenalty = 0.2f;
        public static float AdventurerWinReward = 0.1f;
    }
    public static class OverviewVariables
    {
        public static int ShopItemsSold { get; private set; }
        
        public static int BattlesWon { get; private set; }
        
        public static int CraftingCount { get; private set; }
        
        public static int ItemsBroken { get; private set; }

        public static void SoldItem()
        {
            ShopItemsSold++;
        }

        public static void WonBattle()
        {
            BattlesWon++;
        }

        public static void CraftItem()
        {
            CraftingCount++;
        }

        public static void ItemBroke()
        {
            ItemsBroken++;
        }
    }
}
