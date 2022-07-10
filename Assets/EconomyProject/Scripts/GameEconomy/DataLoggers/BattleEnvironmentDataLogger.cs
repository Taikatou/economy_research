using System.Collections.Generic;
using System.Globalization;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;

namespace EconomyProject.Scripts.GameEconomy.DataLoggers
{
    public struct EBattleEnvironmentSelection
    {
        public EBattleEnvironments BattleEnvironments;
        public int Level;
        public EAdventurerTypes AdventurerTypes;
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
            var rowData = new List<string[]> { new[]{ "Level", "AdventurerType", "MaxCount", "ID" } };
            foreach (var item in levelData)
            {
                var row = new string[] {
                    item.BattleEnvironments.ToString(),
                    item.Level.ToString(),
                    item.AdventurerTypes.ToString(CultureInfo.InvariantCulture),
                    item.ID.ToString()
                };
                rowData.Add(row);
            }
            OutputCsv(rowData, "level_selection_");
        }
    }
}
