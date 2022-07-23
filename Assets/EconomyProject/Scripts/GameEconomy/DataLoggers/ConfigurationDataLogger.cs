using System.Collections.Generic;
using Sirenix.Utilities;

namespace EconomyProject.Scripts.GameEconomy.DataLoggers
{
    
    public class ConfigurationDataLogger : DataLogger
    {
        public Dictionary<string, Dictionary<string, string>> ConfigurationValues;

        protected override void Start()
        {
            base.Start();
            ConfigurationValues = new Dictionary<string, Dictionary<string, string>>();
        }

        public void AddData(string identifier, Dictionary<string, string> configurationData)
        {
            ConfigurationValues.Add(identifier, configurationData);
        }
        
        void OnApplicationQuit()
        {
            PrintLevelData();
        }
        
        private void PrintLevelData()
        {
            var rowData = new List<string[]> { new[]{ "ID" }};
            var added = false;
            foreach (var item in ConfigurationValues)
            {
                var row = new List<string> {
                    item.Key
                };
                row.AddRange(item.Value.Values);
                if (!added)
                {
                    rowData[0].AddRange(item.Value.Keys);
                    added = true;
                }
                rowData.Add(row.ToArray());
            }
            OutputCsv(rowData, "level_configuration_");
        }
    }
}
