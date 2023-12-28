using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{

    [Serializable]
    public class RequestAdventurerSystem : EconomySystem<BaseAdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
    {
        public AdventureRequestLocationSetter adventureRequestLocationSetter;
        public RequestSystem requestSystem;
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Request;

        public override bool CanMove(BaseAdventurerAgent agent)
        {
            return true;
        }
        public override ObsData[] GetObservations(BaseAdventurerAgent agent, BufferSensorComponent[] bufferSensorComponent)
        {
            requestSystem.GetObservations(bufferSensorComponent[0]);
            return Array.Empty<ObsData>();
        }

        protected override void SetChoice(BaseAdventurerAgent agent, EAdventurerAgentChoices input)
        {
            switch (input)
            {
              /*  case EAdventurerAgentChoices.Down:
                    UpDown(agent, -1);
                    break;
                case EAdventurerAgentChoices.Up:
                    UpDown(agent, 1);
                    break;
                case EAdventurerAgentChoices.Select:
                    var resource = adventureRequestLocationSetter.GetRequest(agent);
                    if (resource != null)
                    {
                        agent.RequestTaker.TakeRequest(resource);
                    }
                    break;
                case EAdventurerAgentChoices.Back:
                    AgentInput.ChangeScreen(agent, EAdventurerScreen.Main);
                    break;*/
            }
        }

        public void UpDown(BaseAdventurerAgent agent, int movement)
        {
            adventureRequestLocationSetter.MovePosition(agent, movement);
        }
        

        private List<InputAction> GetRequestInput()
        {
            var output = new List<InputAction>();
            var requests = requestSystem.GetAllCraftingRequests();
            var i = 1;
            foreach (var req in requests)
            {
                output.Add(new InputAction{Action = req.ToString(), ActionNumber = i});
                i++;
            }

            return output;
        }
        
        public override EAdventurerAgentChoices[] GetEnabledInputs(BaseAdventurerAgent agent)
        {
            var inputChoices = new EAdventurerAgentChoices[] { };

            return inputChoices;
        }

        public void Start()
        {
            adventureRequestLocationSetter.requestSystem = requestSystem;
        }
    }
}
