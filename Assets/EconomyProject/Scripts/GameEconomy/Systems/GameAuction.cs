using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public class GameAuction : EconomySystem<AdventurerAgent, AgentScreen>
    {
        [HideInInspector]
        public InventoryItem auctionedItem;

        [HideInInspector]
        public float currentItemPrice;

        [HideInInspector]
        public float currentAuctionTime;

        public float auctionTime = 5.0f;

        public float bidIncrement = 5.0f;

        private AdventurerAgent _currentHighestBidder;

        public float addChance = 0.3f;

        public float maxInventory = 50;

        private List<InventoryItem> _inventoryItems;

        private bool _auctionOn;

        public int ItemCount => _inventoryItems.Count;

        public override float Progress(AdventurerAgent agent) => currentAuctionTime / auctionTime;

        private DataLogger Logger => GetComponent<DataLogger>();

        protected override AgentScreen ActionChoice => AgentScreen.Auction;

        public bool BidOn { get; private set; }

        public bool BidLast { get; private set; }

        public void Reset()
        {
            _inventoryItems?.Clear();
        }

        private void Start()
        {
            _inventoryItems = new List<InventoryItem>();
        }

        public void SetAuctionItem()
        {
            if (!_auctionOn)
            {
                _auctionOn = _inventoryItems.Count > 0;
                if (_auctionOn)
                {
                    var rnd = new System.Random();

                    var index = rnd.Next(_inventoryItems.Count);

                    auctionedItem = _inventoryItems[index];

                    currentAuctionTime = 0.0f;

                    currentItemPrice = auctionedItem.baseBidPrice;
                }

                _currentHighestBidder = null;

                BidOn = false;
                BidLast = false;
            }
        }

        public virtual float AddChance => Mathf.Lerp(addChance, 0.005f, _inventoryItems.Count / maxInventory);

        public void AddAuctionItem(InventoryItem item)
        {
            var rand = new System.Random();
            var randValue = rand.NextDouble();
            var randChance = 0.99;
            if (randValue <= randChance)
            {
                _inventoryItems.Add(item);
            }
        }

        private void FixedUpdate()
        {
            if (CurrentPlayers.Length > 0)
            {
                if (ItemCount > 0)
                {
                    SetAuctionItem();
                    currentAuctionTime += Time.deltaTime;
                    if (currentAuctionTime >= auctionTime)
                    {
                        if (!BidOn)
                        {
                            AuctionOver();
                        }
                        else
                        {
                            BidOn = false;
                        }
                        currentAuctionTime = 0.0f;
                    }
                }
                else
                {
                    ReturnToMain();
                }
                RequestDecisions();
            }
        }

        private void AuctionOver()
        {
            _auctionOn = false;
            _inventoryItems.Remove(auctionedItem);
            if (BidLast)
            {
                Debug.Log("Sold Weapon: " + auctionedItem.itemName + " for " + currentItemPrice);
                _currentHighestBidder.BoughtItem(auctionedItem, currentItemPrice);
                Logger.AddAuctionItem(auctionedItem, currentItemPrice, _currentHighestBidder);
            }
            ReturnToMain();
        }

        protected override void RequestDecisions()
        {
            foreach (var agent in CurrentPlayers)
            {
                var notHighest = !IsHighestBidder(agent);
                if (notHighest)
                {
                    agent.RequestDecision();
                }
            }
        }

        private void ReturnToMain()
        {
            _auctionOn = false;
            foreach (var agent in CurrentPlayers)
            {
                playerInput.ChangeScreen(agent, AgentScreen.Main);
            }
        }

        public override bool CanMove(AdventurerAgent agent)
        {
            if (!_auctionOn)
            {
                return true;
            }

            var valid = _currentHighestBidder && agent;
            var highestBidder = _currentHighestBidder == agent;
            return !(highestBidder && valid);
        }

        public bool IsHighestBidder(AdventurerAgent agent)
        {
            return _currentHighestBidder == agent;
        }

        public void Bid(AdventurerAgent agent)
        {
            if (CurrentPlayers.Contains(agent))
            {
                var newPrice = currentItemPrice + bidIncrement;

                if (!IsHighestBidder(agent) && agent.wallet.Money >= newPrice)
                {
                    _currentHighestBidder = agent;
                    currentItemPrice = newPrice;
                    BidOn = true;
                    BidLast = true;
                }
            }
        }
    }
}
