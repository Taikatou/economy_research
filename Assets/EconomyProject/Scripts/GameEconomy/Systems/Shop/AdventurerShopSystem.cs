using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Inventory;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{

    public delegate void SetChoice(BaseAdventurerAgent agent);
    
    [Serializable]
    public class AdventurerShopSystem : StateEconomySystem<BaseAdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
    {
        public AdventurerShopSubSystem adventurerShopSubSystem;
        public static int ObservationSize => 0;
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Shop;

        public override bool CanMove(BaseAdventurerAgent agent)
        {
            return true;
        }

        public override ObsData[] GetObservations(BaseAdventurerAgent agent, BufferSensorComponent[] bufferSensorComponent)
        {
            adventurerShopSubSystem.GetObservations(bufferSensorComponent[0]);
            
            return new ObsData [] {};
        }

        protected override void SetChoice(BaseAdventurerAgent agent, EAdventurerAgentChoices input)
        {
            switch (input)
            {
         
            }
        }

        private readonly Dictionary<ECraftingChoice, EAdventurerAgentChoices> _mapStuff =
            new()
            {
                { ECraftingChoice.BeginnerSword, EAdventurerAgentChoices.BuyBeginnerSword },
                { ECraftingChoice.IntermediateSword, EAdventurerAgentChoices.BuyIntermediateSword },
                { ECraftingChoice.AdvancedSword, EAdventurerAgentChoices.BuyAdvancedSword },
                { ECraftingChoice.EpicSword, EAdventurerAgentChoices.BuyEpicSword},
                { ECraftingChoice.UltimateSwordOfPower, EAdventurerAgentChoices.BuyUltimateSword}
            };
        
        readonly ECraftingChoice[] _craftingChoices = Enum.GetValues(typeof(ECraftingChoice)).Cast<ECraftingChoice>().ToArray();

        public override EAdventurerAgentChoices[] GetEnabledInputs(BaseAdventurerAgent agent)
        {
            var inputChoices = new List<EAdventurerAgentChoices>();
            foreach (var craftingChoice in _craftingChoices)
            {
                var list = adventurerShopSubSystem.shopCraftingSystem.system.shopSubSubSystem.GetAllUsableItems(false, craftingChoice, true);
                if (list.Count > 0)
                {
                    inputChoices.Add(_mapStuff[list[0].Item1.craftChoice]);
                }
            }
            
            return inputChoices.ToArray();
        }
        
        
    }
}
