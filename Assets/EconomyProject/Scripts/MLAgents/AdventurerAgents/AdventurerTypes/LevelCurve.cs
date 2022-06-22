using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.AdventurerTypes
{
    [System.Serializable]
    public struct ProgressionData
    {
        public int level;
        
        public int expToNextLevel;

        public int damageIncrease;

        public int healthIncrease;
    }
    
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelCurve", order = 1)]
    public class LevelCurve : ScriptableObject
    {
        public ProgressionData[] levelProgressionParts;
    }
}
