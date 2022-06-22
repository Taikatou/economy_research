public enum EAdventurerAgentChoices { None=0,
    Up, Down, Back, Select, SetShop, PurchaseItem }

public enum EAdventurerScreen { Main=EAdventurerAgentChoices.None, Request, 
    Shop, Adventurer, Rest=EAdventurerAgentChoices.Back + 1}