using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.DataLoggers
{
    public struct PurchaseItemData
    {
        public string ItemName;
        public int Price;
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
            _purchaseItems.Add(new PurchaseItemData { });
        }

        private void PrintResourceRequests()
        {
            var rowData = new List<string[]>
            {
                new[] { "Resource", "CompletedTime", "CreationTme", "TakenTime", "Number", "Price" }
            };
            
            OutputCsv(rowData, "request_data");
        }
    }
}
