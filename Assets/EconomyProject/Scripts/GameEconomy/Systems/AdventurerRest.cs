﻿using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public class AdventurerRest : EconomySystem<AdventurerAgent, EAdventurerScreen>
    {
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Rest;
        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }

        public override float[] GetSenses(AdventurerAgent agent)
        {
            throw new System.NotImplementedException();
        }

        public override InputAction[] GetInputOptions(AdventurerAgent agent)
        {
            throw new System.NotImplementedException();
        }
    }
}
