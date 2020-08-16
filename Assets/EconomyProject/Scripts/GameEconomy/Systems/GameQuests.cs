using System.Collections.Generic;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.Inventory.LootBoxes.Generated;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public class GameQuests : EconomySystem<AdventurerAgent, AgentScreen>
    {
        public float spawnTime = 3.0f;

        public bool finiteMonsters = true;

        public bool autoReturn = false;

        public bool multipleLootDrops = true;

        public bool punishFailure = true;

        public GameAuction auction;

        public GenericLootDropTableGameObject lootDropTable;

        public List<ItemMap> rewardMap;

        private bool _shouldReturn;

        private float _currentTime;
        
        private ItemMapper _itemMapper;

        public override float Progress(AdventurerAgent agent)  => _currentTime / spawnTime;

        protected override AgentScreen ActionChoice => AgentScreen.Quest;

        public override bool CanMove(AdventurerAgent agent)
        {
            return !_shouldReturn;
        }

        private void Start()
        {
            lootDropTable.ValidateTable();
            _currentTime = 0.0f;
            _shouldReturn = autoReturn;
            _itemMapper = new ItemMapper(rewardMap);
        }

        private void FixedUpdate()
        {
            if (CurrentPlayers.Length >= 1)
            {
                _currentTime += Time.deltaTime;
                if (_currentTime > spawnTime)
                {
                    if (!finiteMonsters)
                    {
                        foreach (var agent in CurrentPlayers)
                        {
                            RunQuests(agent);
                        }
                    }
                    else
                    {
                        var random = new System.Random();
                        var start2 = random.Next(0, CurrentPlayers.Length - 1);
                        var agent = CurrentPlayers[start2];

                        RunQuests(agent);
                    }

                    if (autoReturn)
                    {
                        _shouldReturn = false;
                        foreach (var agent in CurrentPlayers)
                        {
                            Debug.Log("Exit Quest");
                            // playerInput.ChangeScreen(agent, AgentScreen.Main);
                        }
                        _shouldReturn = true;
                    }

                    _currentTime = 0.0f;
                }
            }
        }

        private void RunQuests(AdventurerAgent agent)
        {
            var item = agent.adventurerInventory.EquipedItem;
            
            var questSuccess = Random.value < (item.itemDetails.damage / 100);
            if(questSuccess)
            {
                var money = GenerateItem(item) / 2;
                agent.wallet.EarnMoney(money);
            }
            else if (punishFailure)
            {
                agent.wallet.LoseMoney(2);
            }
            agent.adventurerInventory.DecreaseDurability();
        }

        private float GenerateItem(UsableItem attackWeapon)
        {
            var totalPrice = 0.0f;

            var r = new System.Random();
            var numSpawns = r.Next(1, 1);
            for (var i = 0; i < numSpawns && (multipleLootDrops || i==0); i++)
            {
                var selectedItem = lootDropTable.PickLootDropItem();
                var generatedItem = UsableItem.GenerateItem(selectedItem.item);
                auction.AddAuctionItem(generatedItem);

                if (generatedItem != null)
                {
                    totalPrice += _itemMapper.GetValue(generatedItem);
                }
            }
            return totalPrice;
        }
    }
}
