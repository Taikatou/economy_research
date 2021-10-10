public enum EAdventurerAgentChoices { None=0, FindRequest, Shop, Adventure, 
    Up, Down, SetShop, PurchaseItem, Back, Select }

public enum EAdventurerScreen { Main=EAdventurerAgentChoices.None, Request=EAdventurerAgentChoices.FindRequest, 
    Shop=EAdventurerAgentChoices.Shop, Adventurer=EAdventurerAgentChoices.Adventure, Rest=EAdventurerAgentChoices.Back + 1}