using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Adventurer
{
    public class AdventurerAgentDropDown : AgentDropDown<BaseAdventurerAgent, EAdventurerScreen>
    {
        public GetCurrentAdventurerAgent currentAdventurerAgent;
        
        protected override GetCurrentAgent<BaseAdventurerAgent> GetCurrentAgent => currentAdventurerAgent;
    }
}
