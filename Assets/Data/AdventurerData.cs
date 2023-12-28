namespace Data
{
    public enum EAdventurerAgentChoices
    {
        None,
        SelectForest,
        SelectMountain,
        SelectSea,
        SelectVolcano,
        BuyBeginnerSword,
        BuyIntermediateSword,
        BuyAdvancedSword,
        BuyEpicSword,
        BuyMasterSword,
        BuyUltimateSword
    }

    public enum EAdventurerScreen { Main=EAdventurerAgentChoices.SelectForest, Request, 
        Shop, Adventurer, Rest=EAdventurerAgentChoices.BuyUltimateSword + 1}
}