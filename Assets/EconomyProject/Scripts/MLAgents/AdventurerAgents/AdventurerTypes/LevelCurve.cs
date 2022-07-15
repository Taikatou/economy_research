using UnityEditor;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.AdventurerTypes
{
    [System.Serializable]
    public struct ProgressionData
    {
        public int level;
        
        public int expRequirement;

        public int damageIncrease;

        public int healthIncrease;
    }
    
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelCurve", order = 1)]
    public class LevelCurve : ScriptableObject
    {
        public ProgressionData[] levelProgressionParts;
    }
    
    [CustomEditor(typeof(LevelCurve))]
    public class TestScriptableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = (LevelCurve)target;
 
            
         
        }
    }
}
