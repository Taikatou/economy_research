using System;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.Interfaces;
using UnityEngine;

namespace EconomyProject.Monobehaviours
{
    public class MainMenuSystemBehaviour : MonoBehaviour, ISetup
    {
        public MainMenuSystem system;
        public void Setup()
        {
            //system.setup();
        }
    }
}
