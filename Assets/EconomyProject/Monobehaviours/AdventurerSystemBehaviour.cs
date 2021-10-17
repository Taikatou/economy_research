using System;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.Interfaces;
using UnityEngine;

namespace EconomyProject.Monobehaviours
{
    public class AdventurerSystemBehaviour : MonoBehaviour, ISetup
    {
        public AdventurerSystem system;

        public void Start()
        {
            system.Start();
        }

        public void Setup()
        {
            system.Setup();
        }
    }
}
