﻿namespace Data
{
    public static class TrainingConfig
    {
        public static bool OnPurchase => true;
        public static float OnPurchaseReward = 0.3f;

        public static bool OnSell => true;
        public static float OnSellReward = 0.2f;

        public static bool OnWin => true;
        public static float OnWinReward = 0.1f;

        public static bool OnResource => true;
        public static float OnResourceReward = 0.1f;

        public static bool OnResourceComplete => true;
        public static float OnResourceCompleteReward = 0.2f;

        public static bool OnCraft => true;
        public static float OnCraftReward = 0.4f;
    }

    public static class UISpec
    {
        public static bool CraftActive { get; set; }
    }

    public static class SystemTraining
    {
        public static int PartySize => 3;
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