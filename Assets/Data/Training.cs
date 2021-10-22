namespace Data
{
    public static class TrainingConfig
    {
        public static bool OnPurchase => true;

        public static bool OnSell => true;

        public static bool OnWin => true;

        public static bool OnResource => true;

        public static bool OnResourceComplete => true;

        public static bool OnCraft => true;
    }

    public static class UISpec
    {
        public static bool craftActive = false;
    }

    public static class SystemTraining
    {
        public static int partySize => 1;
        public static bool removeRequestTime => false;
    }

    public static class ParameterTuning
    {
        public static bool Tune => true;
    }
}