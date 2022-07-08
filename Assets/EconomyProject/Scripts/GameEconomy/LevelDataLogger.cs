using System.Collections.Generic;
using System.Globalization;
using Data;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy
{
    struct LevelData
    {
        public int Level;
        public EAdventurerTypes Adventurer;
        public int StepsFromLast;
    }

    public class LevelDataLogger : DataLogger
    {
        private Dictionary<EAdventurerTypes, List<LevelData>> _levelData;

        protected override void Start()
        {
            base.Start();
            _levelData = new Dictionary<EAdventurerTypes, List<LevelData>>();
        }
        
        public void AddLevelData(int level, EAdventurerTypes adventurer, int stepsFromLast)
        {
            if (!_levelData.ContainsKey(adventurer))
            {
                _levelData.Add(adventurer, new List<LevelData>());
            }
            _levelData[adventurer].Add(new LevelData
            {
                Level = level,
                Adventurer = adventurer,
                StepsFromLast = stepsFromLast
            });
        }

        private void PrintLevelData(List<LevelData> levelData, EAdventurerTypes adventurerTypes)
        {
            var rowData = new List<string[]> { new[]{ "Level", "AdventurerType", "MaxCount" } };
            foreach (var item in levelData)
            {
                var row = new[] {
                    item.Level.ToString(),
                    item.Adventurer.ToString(CultureInfo.InvariantCulture),
                    item.StepsFromLast.ToString()
                };
                rowData.Add(row);
            }
            OutputCsv(rowData, adventurerTypes + "_");
        }
        
        void OnApplicationQuit()
        {
            foreach (var item in _levelData)
            {
                PrintLevelData(item.Value, item.Key);
            }
        }
    }
}
