using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Inventory;
using EconomyProject.Scripts.MLAgents;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy
{
// CODE COURTESY OF https://sushanta1991.blogspot.com/2015/02/how-to-write-data-to-csv-file-in-unity.html

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

    public class DataLogger : MonoBehaviour
    {
        public static int staticLoggerId = 0;

        public string learningEnvironmentId = "agent_id_";

        public int loggerId;

        private int _resetCount = 0;

        private List<AuctionItem> _auctionItems;

        private Dictionary<UsableItem, List<float>> _itemPrices;

        public string CurrentTime
        {
            get
            {
                var endTimer = FindObjectOfType<EndTimerScript>();
                return endTimer ? endTimer.CurrentTime : "";
            }
        }
        public string GetFileName(string fileName)
        {
            var nowStr = DateTime.Now.ToString("_dd_MM_yyyy_HH_mm");
            return fileName + loggerId + nowStr + ".csv";
        }

        private void Start()
        {
            staticLoggerId++;
            loggerId = staticLoggerId;
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

        public void OutputCsv(List<string[]> rowData, string fileName)
        {
            string[][] output = new string[rowData.Count][];

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = rowData[i];
            }

            int length = output.GetLength(0);
            string delimiter = ",";

            StringBuilder sb = new StringBuilder();

            for (int index = 0; index < length; index++)
                sb.AppendLine(string.Join(delimiter, output[index]));


            var filePath = GetPath(fileName);

            var exists = Directory.Exists(filePath);
            if (!exists)
            {
                Directory.CreateDirectory(filePath);   
            }
            
            StreamWriter outStream = File.CreateText(filePath);
            outStream.WriteLine(sb);
            outStream.Close();
        }

        // Following method is used to retrive the relative path as device platform
        private string GetPath(string fileName)
        {
            #if UNITY_EDITOR
                return Application.dataPath + "/CSV/" + GetFileName(fileName);
            #elif UNITY_ANDROID
                return Application.persistentDataPath+"Saved_data.csv";
            #elif UNITY_IPHONE
                return Application.persistentDataPath+"/"+"Saved_data.csv";
            #else
                return Application.dataPath +"/"+"Saved_data.csv";
            #endif
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

        public void Reset()
        {
            _resetCount++;
        }
    }
}