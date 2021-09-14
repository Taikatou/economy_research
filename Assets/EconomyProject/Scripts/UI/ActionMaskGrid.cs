using System;
using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI
{
    public class ActionMaskGrid : MonoBehaviour
    {
        public GetCurrentAdventurerAgent getCurrentAdventurerAgent;
        public GridLayoutGroup gridLayout;
        public ActionMaskButton MaskUI;

        private Dictionary<EAdventurerAgentChoices, ActionMaskButton> _textBoxes;

        private List<EAdventurerAgentChoices> _actions;
        // Start is called before the first frame update
        // Update is called once per frame
        public void Start()
        {
            var actions = Enum.GetValues(typeof(EAdventurerAgentChoices)).Cast<EAdventurerAgentChoices>().ToList();
            foreach (var a in actions)
            {
                var t = Instantiate(MaskUI, gridLayout.transform, true);
                t.textUI.text = a.ToString();
                t.buttonUI.onClick.AddListener(() => ButtonClicked(a));
            }
            
            _textBoxes = new Dictionary<EAdventurerAgentChoices, ActionMaskButton>();
            foreach (var a in actions)
            {
                var t = Instantiate(MaskUI, gridLayout.transform, true);
                _textBoxes.Add(a, t);
            }
        }

        private void ButtonClicked(EAdventurerAgentChoices a)
        {
            getCurrentAdventurerAgent.CurrentAgent.SetAction(a);
        }

        private void Update()
        {
            if (getCurrentAdventurerAgent.CurrentAgent != null)
            {
                var inputs= getCurrentAdventurerAgent.CurrentAgent.GetEnabledInput();
                foreach (var i in inputs)
                {
                    var a = (EAdventurerAgentChoices) i.Input;
                    if (_textBoxes.ContainsKey(a))
                    {
                        _textBoxes[a].textUI.text = i.Enabled.ToString();
                    }
                }   
            }
        }
    }
}
