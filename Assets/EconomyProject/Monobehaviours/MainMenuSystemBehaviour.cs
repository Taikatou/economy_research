using System;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.Interfaces;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Monobehaviours
{
    public class MainMenuSystemBehaviour : MonoBehaviour, ISetup, IGetSystem
    {
        public MainMenuSystem system;
        public void Setup()
        {
            //system.setup();
        }

        public EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> GetSystem => system;
    }
}
