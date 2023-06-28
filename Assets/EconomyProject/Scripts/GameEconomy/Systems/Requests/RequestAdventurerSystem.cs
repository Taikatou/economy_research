using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{

    [Serializable]
    public class RequestAdventurerSystem : EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
    {
        public AdventureRequestLocationSetter adventureRequestLocationSetter;
        public RequestSystem requestSystem;
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Request;

        public static int ObservationSize =>
            CraftingResourceRequest.SensorCount + CraftingResourceRequest.SensorCount + 32;
        
        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }
        public override ObsData[] GetObservations(AdventurerAgent agent, BufferSensorComponent[] bufferSensorComponent)
        {
            var input = new List<ObsData>();
            
            var itemList = new Dictionary<ECraftingResources, CraftingResourceRequest>
            {
                {ECraftingResources.Gem, null},
                {ECraftingResources.Metal, null},
                {ECraftingResources.Wood, null},
                {ECraftingResources.DragonScale, null},
            };
            // todo use buffer
            foreach (var i in agent.requestTaker.GetItemList())
            {
                itemList[i.Resource] = i;
            }
            var senseA = CraftingResourceRequest.GetObservations(itemList);
            input.AddRange(senseA);
            var senseB = requestSystem.GetObservations(agent, null);
            input.AddRange(senseB);
            return input.ToArray();
        }

        protected override void SetChoice(AdventurerAgent agent, EAdventurerAgentChoices input)
        {
            switch (input)
            {
                case EAdventurerAgentChoices.Down:
                    UpDown(agent, -1);
                    break;
                case EAdventurerAgentChoices.Up:
                    UpDown(agent, 1);
                    break;
                case EAdventurerAgentChoices.Select:
                    var resource = adventureRequestLocationSetter.GetRequest(agent);
                    if (resource != null)
                    {
                        agent.requestTaker.TakeRequest(resource);
                    }
                    break;
                case EAdventurerAgentChoices.Back:
                    AgentInput.ChangeScreen(agent, EAdventurerScreen.Main);
                    break;
            }
        }

        public void UpDown(AdventurerAgent agent, int movement)
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
        
        public override EnabledInput[] GetEnabledInputs(AdventurerAgent agent)
        {
            var inputChoices = new[]
            {
                EAdventurerAgentChoices.Select,
                EAdventurerAgentChoices.Back,
                EAdventurerAgentChoices.Up,
                EAdventurerAgentChoices.Down
            };
            var outputs = EconomySystemUtils<EAdventurerAgentChoices>.GetInputOfType(inputChoices);

            return outputs;
        }

        public void Start()
        {
            adventureRequestLocationSetter.requestSystem = requestSystem;
        }
    }
}
