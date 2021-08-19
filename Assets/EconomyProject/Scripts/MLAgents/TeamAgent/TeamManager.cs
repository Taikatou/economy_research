using System;
using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.TeamAgent
{
    public class TeamManager : MonoBehaviour
    {
        private Dictionary<Guid, string> PlayerTeamMap { get; set; }

        private HashSet<string> TeamHashSet { get; set; }

        public void Start()
        {
            PlayerTeamMap = new Dictionary<Guid, string>();
            TeamHashSet = new HashSet<string>();
        }

        public bool AddToTeam(string teamName, Guid playerID)
        {
            var leader = false;
            if (!TeamHashSet.Contains(teamName))
            {
                leader = true;
                TeamHashSet.Add(teamName);
            }

            if (PlayerTeamMap.ContainsKey(playerID))
            {
                PlayerTeamMap.Remove(playerID);
            }
            PlayerTeamMap.Add(playerID, teamName);

            return leader;
        }
    }
}
