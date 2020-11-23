namespace EconomyProject.Scripts.UI
{
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
