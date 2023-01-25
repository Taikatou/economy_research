using System.Collections.Generic;
using System.Globalization;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;

namespace EconomyProject.Scripts.GameEconomy.DataLoggers
{
    public struct EBattleEnvironmentSelection
    {
        public string ConfigurationID;
        public EBattleEnvironments BattleEnvironments;
        public int Level;
        public EAdventurerTypes AdventurerTypes;
        public int AverageStepCount;
        public int ID;
    }
    public class BattleEnvironmentDataLogger : DataLogger
    {
        private List<EBattleEnvironmentSelection> _environmentSelection;

        protected override void Start()
        {
            base.Start();
            _environmentSelection = new List<EBattleEnvironmentSelection>();
        }

        public void AddEnvironmentSelection(EBattleEnvironmentSelection environment)
        {
            _environmentSelection.Add(environment);   
        }
        
        void OnApplicationQuit()
        {
            PrintLevelData(_environmentSelection);
        }

        private void PrintLevelData(List<EBattleEnvironmentSelection> levelData)
        {
            var rowData = new List<string[]> { new[]{ "ConfigurationID", "BattleEnvironment", "Level", "AdventurerType", "AverageStepCount", "ID" } };
            foreach (var item in levelData)
            {
                var row = new string[] {
                    item.ConfigurationID,
                    item.BattleEnvironments.ToString(),
                    item.Level.ToString(),
                    item.AdventurerTypes.ToString(CultureInfo.InvariantCulture),
                    item.AverageStepCount.ToString(),
                    item.ID.ToString()
                };
                rowData.Add(row);
            }
            OutputCsv(rowData, "level_selection_");
        }
    }
}
