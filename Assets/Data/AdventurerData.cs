namespace Data
{
    public enum EAdventurerAgentChoices { None=0,
        Up, Down, Back, Select }

    public enum EAdventurerScreen { Main=EAdventurerAgentChoices.None, Request, 
        Shop, Adventurer, Rest=EAdventurerAgentChoices.Back + 1}
}