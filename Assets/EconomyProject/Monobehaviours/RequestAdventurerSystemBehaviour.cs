﻿using System;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using UnityEngine;

namespace EconomyProject.Monobehaviours
{
    public class RequestAdventurerSystemBehaviour : MonoBehaviour
    {
        public RequestAdventurerSystem system;

        public void Start()
        {
            system.Start();
        }
    }
}
