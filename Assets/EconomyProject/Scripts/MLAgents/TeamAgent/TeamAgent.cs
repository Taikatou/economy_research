using System;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.MLAgents.TeamAgent
{
    public class TeamAgent : AdventurerAgent
    {
        public string teamName;
        public TeamManager teamManager;

        private Guid PlayerId { get; set; }

        public void Start()
        {
            PlayerId = Guid.NewGuid();
            teamManager.AddToTeam(teamName, PlayerId);
        }
    }
}
