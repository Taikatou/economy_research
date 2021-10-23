using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.Adventurer;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.Interfaces;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public enum EAdventureStates { OutOfBattle, InQueue, InBattle }
    
    [Serializable]
    public class AdventurerSystem : EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>, ISetup
    {
        public BattleSubSystem battleSubSystem;
        public static int SensorCount => 1;
        public Dictionary<AdventurerAgent, EAdventureStates> adventureStates;

        public static int ObservationSize => SensorCount + BattleSubSystem.SensorCount + AdventurerLocationSelect.SensorCount;
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Adventurer;

        public AdventurerLocationSelect locationSelect;
        public BattleLocationSelect battleLocationSelect;

        public TravelSubSystem travelSubsystem;

        public void Start()
        {
            battleSubSystem = new BattleSubSystem(travelSubsystem, SetAdventureState);
            adventureStates = new Dictionary<AdventurerAgent, EAdventureStates>();
        }

        public void Setup()
		{
            battleSubSystem.Setup();
            adventureStates.Clear();
        }

        public EAdventureStates GetAdventureStates(AdventurerAgent agent)
        {
            var toReturn = EAdventureStates.OutOfBattle;
            if (agent != null)
            {
                if (!adventureStates.ContainsKey(agent))
                {
                    adventureStates.Add(agent, EAdventureStates.OutOfBattle);
                }

                toReturn = adventureStates[agent];
            }
            return toReturn;
        }

        private void SetAdventureState(AdventurerAgent agent, EAdventureStates state)
        {
            if (!adventureStates.ContainsKey(agent))
            {
                adventureStates.Add(agent, state);
            }
            else
            {
                adventureStates[agent] = state;
            }
            Refresh(agent);
        }
        
        public override bool CanMove(AdventurerAgent agent)
        {
            return GetAdventureStates(agent) == EAdventureStates.OutOfBattle;
        }

        public override ObsData[] GetObservations(AdventurerAgent agent)
        {
            ObsData[] GetSubsystemData(AdventurerAgent agent)
            {
                ObsData[] toReturn;
                var i = battleLocationSelect.GetCurrentLocation(agent);
                var subSystem = battleSubSystem.GetSubSystem(agent);
                if (subSystem != null)
                {
                    toReturn = subSystem.GetSubsystemObservations(i);
                }
                else
                {
                    toReturn = BlankArray(BattleSubSystemInstance<AdventurerAgent>.SensorCount);
                }

                return toReturn;
            }
            ObsData[] BlankArray(int sensorCount)
            {
                return new ObsData[sensorCount];
            }
            
            var state = GetAdventureStates(agent);
            var battleState = new List<ObsData> { new ObsData {data=(float) state, name="AdventureState"}};

            var output = state == EAdventureStates.InBattle
                ? GetSubsystemData(agent)
                : BlankArray(BattleSubSystemInstance<AdventurerAgent>.SensorCount);
            battleState.AddRange(output);

            var output2 = state == EAdventureStates.OutOfBattle
                ? locationSelect.GetTravelObservations(agent)
                : BlankArray(AdventurerLocationSelect.SensorCount);
            battleState.AddRange(output2);
            return battleState.ToArray();
        }

        protected override void SetChoice(AdventurerAgent agent, EAdventurerAgentChoices input)
        {
            var validInput = ValidInput(agent, input);
            if (validInput)
            {
                switch(input)
                {
                    case EAdventurerAgentChoices.Back:
                        AgentInput.ChangeScreen(agent, EAdventurerScreen.Main);
                        break;
                    case EAdventurerAgentChoices.Select:
                        Select(agent);
                        break;
                    case EAdventurerAgentChoices.Up:
                        UpDown(agent, 1);
                        break;
                    case EAdventurerAgentChoices.Down:
                        UpDown(agent, -1);
                        break;
                }
            }
        }

        public void Select(AdventurerAgent agent)
        {
            switch (GetAdventureStates(agent))
            {
                case EAdventureStates.InBattle:
                    var action = battleLocationSelect.GetBattleAction(agent);
                    battleSubSystem.SelectBattle(agent, action);
                    break;
                case EAdventureStates.OutOfBattle:
                    var location = locationSelect.GetBattle(agent);
                    battleSubSystem.StartBattle(agent, location);
                    break;
            }
        }

        public void UpDown(AdventurerAgent agent, int movement)
        {
            var location = GetAdventureStates(agent) == EAdventureStates.InBattle
                ? (LocationSelect<AdventurerAgent>) battleLocationSelect
                : (LocationSelect<AdventurerAgent>) locationSelect;
            location.MovePosition(agent, movement);
        }

        public int GetBattleCount()
        {
            var count = 0;
            foreach (var entry in battleSubSystem.battleSystems)
            {
                count++;
            }
            return count;
        }

        public override EnabledInput[] GetEnabledInputs(AdventurerAgent agent)
        {
            var inputChoices = new[]
            {
                EAdventurerAgentChoices.Up,
                EAdventurerAgentChoices.Down,
                EAdventurerAgentChoices.Select,
                EAdventurerAgentChoices.Back
            };

            var outputs = EconomySystemUtils<EAdventurerAgentChoices>.GetInputOfType(inputChoices);

            return outputs;
        }
    }
}
