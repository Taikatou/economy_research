﻿using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Craftsman;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Craftsman.Crafting
{
    public class CraftingSlider : MonoBehaviour
    {
        public Slider slider;
        public CraftingSystem craftingSystem;
        public GetCurrentShopAgent getCurrentAgent;

        private void Update()
        {
            Time.timeScale = 1.0f;
            slider.value = craftingSystem.Progress(getCurrentAgent.CurrentAgent);
        }
    }
}