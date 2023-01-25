namespace Data
{
    public static class TrainingConfig
    {
        public static bool OnPurchase => false;
        public static readonly float OnPurchaseReward = 0.3f;

        public static bool OnSell => false;
        public static readonly float OnSellReward = 0.2f;

        public static bool OnWin => true;
        public static readonly float OnWinReward = 0.1f;

        public static bool OnLose => true;
        public static readonly float OnLoseReward = -0.1f;

        public static bool OnResource => false;
        public static readonly float OnResourceReward = 0.1f;

        public static bool OnResourceComplete => false;
        public static readonly float OnResourceCompleteReward = 0.2f;

        public static bool OnCraft => false;
        public static readonly float OnCraftReward = 0.4f;

        public static bool OnLevelUp = true;
        public static readonly float OnLevelUpReward = 0.1f;

        public static EAdventurerScreen StartScreen => EAdventurerScreen.Adventurer;

        public static bool PunishMovement => false;
        public static readonly float OnPunishMovementReward = 0.0005f;
    }

    public static class UISpec
    {
        public static bool CraftActive { get; set; }
    }

    public static class SystemTraining
    {
        public static int PartySize => 2;
        public static bool RemoveRequestTime => false;

        public static readonly bool IncludeShop = false;
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