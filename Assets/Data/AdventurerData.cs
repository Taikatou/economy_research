public enum EAdventurerAgentChoices { None=0, MainMenu, FindRequest, Shop, Adventure, TakeRequest, 
    AForest, ASea, AMountain, AVolcano, BAttack, BHeal, 
    Up, Down, SetShop, PurchaseItem, Back }

public enum EAdventurerScreen { Main=EAdventurerAgentChoices.MainMenu, Request=EAdventurerAgentChoices.FindRequest, 
    Shop=EAdventurerAgentChoices.Shop, Adventurer=EAdventurerAgentChoices.Adventure, Rest=EAdventurerAgentChoices.Back + 1}