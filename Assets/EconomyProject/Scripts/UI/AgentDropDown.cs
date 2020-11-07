using System;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.MLAgents;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI
{
    public abstract class AgentDropDown<TAgent, T> : MonoBehaviour where TAgent : AgentScreen<T>
    {
        public Dropdown dropDown;
        private bool _setOption;

        private TAgent[] AgentList => GetCurrentAgent.GetAgents;
        protected abstract GetCurrentAgent<TAgent> GetCurrentAgent { get; }

        private DateTime _lastUpdate;
        
        // Update is called once per frame
        protected virtual void Update()
        {
            if (GetCurrentAgent.LastUpdated != _lastUpdate)
            {
                SetDropdown();
            }
        }

        private void SetDropdown()
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
                }
            }
        }

        protected virtual void UpdateAgent(TAgent agent)
        {
            GetCurrentAgent.UpdateAgent(agent);
        }
    }
}
