using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI
{
    public class ActionMaskGrid : MonoBehaviour
    {
        public GetCurrentAgentAggregator getCurrentAgentAggregator;
        public GridLayoutGroup gridLayout;
        public ActionMaskButton maskUI;

        private Dictionary<int, ActionMaskButton> _textBoxes;

        private List<EAdventurerAgentChoices> _actions;

        private bool _cachedCraftActive;
        
        // Start is called before the first frame update
        // Update is called once per frame
        public void Start()
        {
            _cachedCraftActive = UISpec.CraftActive;
            UpdateMenus();
        }

        private void UpdateMenus()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            _textBoxes = new Dictionary<int, ActionMaskButton>();
            if (_cachedCraftActive)
            {
                InitMenus<EShopAgentChoices>();
            }
            else
            {
                InitMenus<EAdventurerAgentChoices>();
            }
        }

        private void AddTextBox<T>(List<T> actions) where T : Enum
        {
            foreach (var a in actions)
            {
                var t = Instantiate(maskUI, gridLayout.transform, true);
                _textBoxes.Add(Convert.ToInt32(a), t);
            }
        }

        private void InitMenus<T> () where T : Enum
        {
            var actions = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            var x = (float) 1920 / actions.Count;
            gridLayout.cellSize = new Vector2(x, gridLayout.cellSize.y);
            foreach (var a in actions)
            {
                var t = Instantiate(maskUI, gridLayout.transform, true);
                var text = a.ToString();

                t.textUI.text = text;
                t.buttonUI.onClick.AddListener(() => ButtonClicked(a));
            }

            AddTextBox(actions);
        }

        private void ButtonClicked<T> (T a) where T : Enum
        {
            getCurrentAgentAggregator.CurrentAgent.SetAction(Convert.ToInt32(a));
        }

        private void Update()
        {
            var newToggle = UISpec.CraftActive;
            if (_cachedCraftActive != newToggle)
            {
                Debug.Log(newToggle);
                _cachedCraftActive = newToggle;
                UpdateMenus();
            }
            else if (getCurrentAgentAggregator.CurrentAgent != null)
            {
                var inputs= getCurrentAgentAggregator.CurrentAgent.GetEnabledInput();
                foreach (var i in inputs)
                {
                    var a = Convert.ToInt32(i.Input);
                    if (_textBoxes.ContainsKey(a))
                    {
                        _textBoxes[a].textUI.text = i.Enabled.ToString();
                    }
                }   
            }
        }
    }
}
