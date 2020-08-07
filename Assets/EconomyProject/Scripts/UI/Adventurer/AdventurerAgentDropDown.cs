﻿using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Adventurer
{
    public class AdventurerAgentDropDown : AgentDropDown<AdventurerAgent, AgentScreen>
    {
        public GetCurrentAdventurerAgent currentAdventurerAgent;
        
        protected override GetCurrentAgent<AdventurerAgent> GetCurrentAgent => currentAdventurerAgent;
    }
}
