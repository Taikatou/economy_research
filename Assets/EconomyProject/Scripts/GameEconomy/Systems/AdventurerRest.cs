﻿using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public class AdventurerRest : EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerScreen>
    {
        public override int ObservationSize { get; }
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Rest;
        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }

        public override float[] GetObservations(AdventurerAgent agent)
        {
            throw new System.NotImplementedException();
        }
    }
}
