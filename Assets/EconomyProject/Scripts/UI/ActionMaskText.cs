using System;
using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI
{
    public class ActionMaskText : MonoBehaviour
    {
        public GetCurrentAdventurerAgent getCurrentAdventurerAgent;
        public GridLayoutGroup gridLayout;
        public Text MaskUI;

        private Dictionary<EAdventurerAgentChoices, Text> _textBoxes;

        private List<EAdventurerAgentChoices> _actions;
        // Start is called before the first frame update
        // Update is called once per frame
        public void Start()
        {
            var actions = Enum.GetValues(typeof(EAdventurerAgentChoices)).Cast<EAdventurerAgentChoices>().ToList();
            foreach (var a in actions)
            {
                var t = Instantiate(MaskUI, gridLayout.transform, true);
                t.text = a.ToString();
            }
            
            _textBoxes = new Dictionary<EAdventurerAgentChoices, Text>();
            foreach (var a in actions)
            {
                var t = Instantiate(MaskUI, gridLayout.transform, true);
                _textBoxes.Add(a, t);
            }
        }

        void Update()
        {
            if (getCurrentAdventurerAgent.CurrentAgent != null)
            {
                var inputs= getCurrentAdventurerAgent.CurrentAgent.GetEnabledInput();
                foreach (var i in inputs)
                {
                    var a = (EAdventurerAgentChoices) i.Input;
                    if (_textBoxes.ContainsKey(a))
                    {
                        _textBoxes[a].text = i.Enabled.ToString();
                    }
                }   
            }
        }
    }
}
