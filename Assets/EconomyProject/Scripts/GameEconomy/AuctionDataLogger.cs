using System.Collections.Generic;
using System.Globalization;
using EconomyProject.Scripts.MLAgents;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Inventory;

namespace EconomyProject.Scripts.GameEconomy
{
    struct AuctionItem
    {
        public UsableItem item;
        public float price;
        public int agentId;
        public string currentTime;

        public string Name => item.itemDetails.itemName;

        public AuctionItem(UsableItem item, float price, int agentId, string currentTime) : this()
        {
            this.item = item;
            this.price = price;
            this.agentId = agentId;
            this.currentTime = currentTime;
        }
    }
    
    public class AuctionDataLogger : DataLogger
    {
        private int _resetCount = 0;
        private List<AuctionItem> _auctionItems;
        private Dictionary<UsableItem, List<float>> _itemPrices;

        protected override void Start()
        {
            _auctionItems = new List<AuctionItem>();
            _itemPrices = new Dictionary<UsableItem, List<float>>();
        }

        public void AddAuctionItem(UsableItem item, float price, AdventurerAgent agent)
        {
            AuctionItem newItem = new AuctionItem(item, price, agent.GetComponent<AgentID>().agentId, CurrentTime);
            _auctionItems.Add(newItem);

            if (!_itemPrices.ContainsKey(item))
            {
                _itemPrices.Add(item, new List<float>());
            }
            _itemPrices[item].Add(price);
        }
        
        public void Reset()
        {
            _resetCount++;
        }
        
        void OnApplicationQuit()
        {
            var rowData = new List<string[]> { new[]{ "Item Name", "Item Price", "AgentID", "Event Time" } };
            foreach (var item in _auctionItems)
            {
                var row = new[] {
                    item.Name,
                    item.price.ToString(CultureInfo.InvariantCulture),
                    item.agentId.ToString(),
                    item.currentTime
                };
                rowData.Add(row);
            }
            OutputCsv(rowData, learningEnvironmentId);
        }
    }
}
