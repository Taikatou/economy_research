using System;
using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.Interfaces;
using UnityEngine;

namespace EconomyProject.Monobehaviours
{
    public class RequestAdventurerSystemBehaviour : MonoBehaviour, ISetup
    {
        public RequestAdventurerSystem system;

        public void Start()
        {
            system.Start();
        }

        public void Setup()
        {
            // system.setup();
        }
    }
}
