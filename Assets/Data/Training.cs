namespace Data
{
    public static class TrainingConfig
    {
        public static bool OnPurchase => true;
        public static readonly float OnPurchaseReward = 0.3f;

        public static bool OnSell => true;
        public static readonly float OnSellReward = 0.2f;

        public static bool OnWin => true;
        public static readonly float OnWinReward = 0.03f;

        public static bool OnLose => true;
        public static readonly float OnLoseReward = -0.01f;

        public static bool LoseMoney => true;
        public static readonly int MoneyToLose = 1;

        public static bool OnResource => false;
        public static readonly float OnResourceReward = 0.1f;

        public static bool OnResourceComplete => false;
        public static readonly float OnResourceCompleteReward = 0.2f;

        public static bool OnCraft => false;
        public static readonly float OnCraftReward = 0.4f;

        public static bool OnLevelUp => false;
        public static readonly float OnLevelUpReward = 0.1f;

        public static EAdventurerScreen StartScreen => SystemTraining.IncludeShop? EAdventurerScreen.Main: EAdventurerScreen.Adventurer;

        public static bool PunishMovement => false;
        public static readonly float OnPunishMovementReward = 0.0005f;
        
        public static bool SkipShopSetup => false;
        
        public static bool RequireConfirmation => false;

        public static bool AdventurerNoRequestMenu => true;

        public static int MaxPrice => 250;
        
        public static bool IgnoreResources => false;

        public static bool SubmitDirectToShop => true;

        public static bool SimpleAdventurerSystem => true;
    }

    public static class UISpec
    {
        public static bool CraftActive { get; set; }
    }

    public static class SystemTraining
    {
        public static int PartySize => 1;
        public static bool RemoveRequestTime => false;

        public static readonly bool IncludeShop = true;
    }

    public static class ParameterTuning
    {
        public static bool Tune => true;
    }

    public static class Observations
    {
        public static bool DebugObs => false;
        public static bool CanViewConstant => false;
    }
}