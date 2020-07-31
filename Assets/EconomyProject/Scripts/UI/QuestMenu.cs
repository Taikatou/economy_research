using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.UI.Adventurer;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI
{
    public class QuestMenu : MonoBehaviour
    {
        public Slider slider;

        public UiAccessor accessor;

        public GetCurrentAdventurerAgent currentAdventurerAgent;

        private void Update()
        {
            slider.value = Progress;
        }

        public RectTransform FindContent(Scrollbar scrollViewObject)
        {
            RectTransform retVal = null;
            Transform[] temp = scrollViewObject.GetComponentsInChildren<Transform>();
            foreach (var child in temp)
            {
                if (child.name == "Content")
                {
                    retVal = child.gameObject.GetComponent<RectTransform>();
                }
            }
            return retVal;
        }

        public float Progress => accessor.GameQuests.Progress(currentAdventurerAgent.CurrentAgent);
    }
}
