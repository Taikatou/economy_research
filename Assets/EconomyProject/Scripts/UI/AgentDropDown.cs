using System;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI
{
    public abstract class AgentDropDown<TAgent, T> : MonoBehaviour where TAgent : AgentScreen<T>
    {
        public Color OnSelected = Color.green;
        public Color OriginalColor = Color.white;

        public Image dropdownImage;
        public Dropdown dropDown;
        private bool _setOption;

        private bool _selected;

        private TAgent[] AgentList => GetCurrentAgent.GetAgents;
        protected abstract GetCurrentAgent<TAgent> GetCurrentAgent { get; }

        private DateTime _lastUpdate;

		//System to update when we change the agent
		public RequestSystem requestSystem;
		public ShopCraftingSystemBehaviour shopCraftingSystemBehaviour;

		// Update is called once per frame
		protected virtual void Update()
        {
            if (GetCurrentAgent.LastUpdated != _lastUpdate || dropDown.options.Count != AgentList.Length)
			{
				SetDropdown();
            }

            if (Selected())
            {
                dropdownImage.color = OnSelected;
                _selected = true;
            }
            else if (_selected)
            {
                _selected = false;
                dropdownImage.color = OriginalColor;
            }
        }

        public void SetDropdown()
        {
            dropDown.ClearOptions();
            
            foreach (var agent in AgentList)
            {
                var agentId = agent.GetComponent<AgentID>();
                if (agentId != null)
                {
                    var agentIdStr = agentId.agentId.ToString();
                    dropDown.options.Add(new Dropdown.OptionData(agentIdStr));
                }
            }

            if (!_setOption && AgentList.Length > 0)
            {
                _setOption = true;
                dropDown.value = 0;
            }

            _lastUpdate = GetCurrentAgent.LastUpdated;
        }

        private void Start()
        {
            SetDropdown();
            
            dropDown.onValueChanged.AddListener(delegate {
                HandleChange();
            });
        }

        private void HandleChange()
        {
            var id = dropDown.options[dropDown.value].text;
			foreach (var agent in AgentList)
            {
                var agentId = agent.GetComponent<AgentID>();
                if (agentId.agentId == int.Parse(id))
                {
                    UpdateAgent(agent);
					UpdateSystems();
				}
			}
        }

        protected virtual void UpdateAgent(TAgent agent)
        {
            GetCurrentAgent.UpdateAgent(agent);
		}

		protected void UpdateSystems()
		{
			requestSystem.Refresh();
			shopCraftingSystemBehaviour.system.shopSubSubSystem.Refresh();
		}

        protected virtual bool Selected()
        {
            return false;
        }
	}
}
