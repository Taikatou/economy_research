using System;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI
{
    public class InputModeText : MonoBehaviour
    {
        public GetCurrentShopAgent shopAgent;
        public ShopInput shopInput;
        public GetCurrentAdventurerAgent adventurerAgent;
        
        public AdventurerInput adventurerInput;
        public ToggleButton toggleButton;
        public Text inputText;

        private EconomySystem<ShopAgent, EShopScreen> _cachedShopSystem;
        private EconomySystem<AdventurerAgent, EAdventurerScreen> _cachedAdventurerSystem;

        private DateTime _cachedTime;
        private bool _cachedToggle;

        // Update is called once per frame
        private void Update()
        {
            CheckCache();
            
            if (toggleButton.craftActive)
            {
                CheckEconomy(shopInput, ref _cachedShopSystem, shopAgent.CurrentAgent);
            }
            else
            {
                CheckEconomy(adventurerInput, ref _cachedAdventurerSystem, adventurerAgent.CurrentAgent);
            }
        }

        private void CheckCache()
        {
            if (_cachedToggle != toggleButton.craftActive)
            {
                _cachedShopSystem = null;
                _cachedAdventurerSystem = null;
                _cachedToggle = toggleButton.craftActive;
                _cachedTime = DateTime.MinValue;
            }
        }

        private void CheckEconomy<TAgent, TScreen>(AgentInput<TAgent, TScreen> agentSystem, 
            ref EconomySystem<TAgent, TScreen> cachedEconomySystem,
            TAgent agent) where TAgent : AgentScreen<TScreen> where TScreen : Enum
        {
            var economySystem = agentSystem.GetEconomySystem(agent);
            var refreshTime = economySystem.GetRefreshTime(agent);
            if (economySystem != cachedEconomySystem || _cachedTime != refreshTime)
            {
                cachedEconomySystem = economySystem;
                _cachedShopSystem = null;
                    
                inputText.text = SetupText(economySystem, agent);
                _cachedTime = refreshTime;
            }
        }

        private static string SetupText<TAgent, TScreen>(EconomySystem<TAgent, TScreen> economy, TAgent agent) where TAgent : AgentScreen<TScreen> where TScreen : Enum
        {
            var options = economy.GetInputOptions(agent);
            var outputText = "";
            foreach(var option in options)
            {
                outputText += option.ActionNumber + ": "+ option.Action + "\t" ;
            }
            return outputText;
        }
    }
}
