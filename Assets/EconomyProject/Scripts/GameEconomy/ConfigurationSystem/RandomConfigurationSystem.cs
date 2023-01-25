using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.GameEconomy.DataLoggers;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.ConfigurationSystem
{
    public class RandomConfigurationSystem : MonoBehaviour
    {
        public ConfigurationDataLogger configurationLogger;

        private static float SwordsmanExpBonus = 1.0f;

        private static float MageExpBonus = 1.0f;

        private static float BrawlerExpBonus = 1.0f;

        public float lowerExpRange = 0.8f;

        public float highExpRange = 1.2f;
        
        public static System.Guid Guid = System.Guid.NewGuid();

        public void UpdateValues()
        {
            var configurationValues = new Dictionary<string, string>();
            
            var rand = new System.Random();
            var randVal = rand.NextDouble() * (highExpRange - lowerExpRange);
            SwordsmanExpBonus = (float) (lowerExpRange + randVal);
            SwordsmanExpBonus /= 2;
            configurationValues.Add("SwordsmanExpBonus", SwordsmanExpBonus.ToString());
            
            randVal = rand.NextDouble() * (highExpRange - lowerExpRange);
            MageExpBonus = (float) (lowerExpRange + randVal);
            MageExpBonus /= 2;
            configurationValues.Add("MageExpBonus", MageExpBonus.ToString());
            
            randVal = rand.NextDouble() * (highExpRange - lowerExpRange);
            BrawlerExpBonus = (float) (lowerExpRange + randVal);
            BrawlerExpBonus /= 2;
            configurationValues.Add("TankExpBonus", BrawlerExpBonus.ToString());
            
            Guid = System.Guid.NewGuid();

            configurationLogger.AddData(Guid.ToString(), configurationValues);
        }

        private int _steps = 0;

        public void FixedUpdate()
        {
            _steps++;
            if (_steps >= 35000)
            {
                UpdateValues();
                var agents = FindObjectsOfType<Agent>();
                foreach (var agent in agents)
                {
                    agent.EndEpisode();
                }

                _steps = 0;
            }
        }

        public static float GetExpBonus(EAdventurerTypes agent)
        {
            switch (agent)
            {
                case EAdventurerTypes.Brawler:
                    return BrawlerExpBonus;
                case EAdventurerTypes.Mage:
                    return MageExpBonus;
                case EAdventurerTypes.Swordsman:
                    return SwordsmanExpBonus;
                default:
                    return 0;
            }
        }
    }
}
