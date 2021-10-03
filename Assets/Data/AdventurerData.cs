public enum EAdventurerAgentChoices { None=0, MainMenu, FindRequest, Shop, Adventure, 
    Up, Down, SetShop, PurchaseItem, Back, Select }

public enum EAdventurerScreen { Main=EAdventurerAgentChoices.MainMenu, Request=EAdventurerAgentChoices.FindRequest, 
    Shop=EAdventurerAgentChoices.Shop, Adventurer=EAdventurerAgentChoices.Adventure, Rest=EAdventurerAgentChoices.Back + 1}