using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.GameEconomy.ConfigurationSystem;
using Sirenix.Utilities;

namespace EconomyProject.Scripts.GameEconomy.DataLoggers
{
    
    public class ConfigurationDataLogger : DataLogger
    {
        public RandomConfigurationSystem randomConfig;
        private Dictionary<string, Dictionary<string, string>> _configurationValues;

        protected override void Start()
        {
            base.Start();
            _configurationValues = new Dictionary<string, Dictionary<string, string>>();
            randomConfig.UpdateValues();
        }

        public void AddData(string identifier, Dictionary<string, string> configurationData)
        {
            _configurationValues.Add(identifier, configurationData);
        }
        
        void OnApplicationQuit()
        {
            PrintLevelData();
        }
        
        private void PrintLevelData()
        {
            bool header = false;
            var rowData = new List<string[]>();
            foreach (var item in _configurationValues)
            {
                if (!header)
                {
                    header = true;
                    var head = new List<string> {"DateTime"};
                    head.AddRange(item.Value.Keys);
                    rowData.Add(head.ToArray());
                }
                var row = new List<string> {
                    item.Key
                };
                
                row.AddRange(item.Value.Values);
                
                rowData.Add(row.ToArray());
            }
            OutputCsv(rowData, "level_configuration_");
        }
    }
}
