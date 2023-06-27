using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.DataLoggers
{
    public struct PurchaseItemData
    {
        public string ItemName;
        public int Price;
        public float SaleTime;
        public float EnterTime;
    }
    public class PurchaseItemDataLogger : DataLogger
    {
        private List<PurchaseItemData> _purchaseItems;
        protected override void Start()
        {
            base.Start();
            _purchaseItems = new List<PurchaseItemData>();
        }

        void OnApplicationQuit()
        {
            PrintResourceRequests();
        }

        public void AddPurchaseItem(PurchaseItemData resourceRequest)
        {
            _purchaseItems.Add(resourceRequest);
        }

        private void PrintResourceRequests()
        {
            var rowData = new List<string[]>
            {
                new[] { "Item", "Price", "SaleTime" }
            };

            foreach (var item in _purchaseItems)
            {
                var row = new[]
                {
                    item.ItemName,
                    item.Price.ToString(),
                    item.SaleTime.ToString(),
                    item.EnterTime.ToString()
                };
                rowData.Add(row);
            }

            OutputCsv(rowData, "purchase_data");
        }
    }
}
