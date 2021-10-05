using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public class AdventurerRest : EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerScreen>
    {
        public override EAdventurerScreen ActionChoice => (EAdventurerScreen.Rest);
        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }

        public override ObsData[] GetObservations(AdventurerAgent agent)
        {
            throw new System.NotImplementedException();
        }
    }
}
