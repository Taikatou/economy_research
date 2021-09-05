using System;
using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy
{
    public static class AdventurerEconomySystemUtils
    {
        public static EnabledInput[] GetInputOfType(bool enabled)
        {
            var actions = Enum.GetValues(typeof(EAdventurerAgentChoices));
            var output = new EnabledInput[actions.Length];
            foreach (var action in actions)
            {
                var i = (int) action;
                output[i] = new EnabledInput { Input = i, Enabled = enabled };
            }
            return output;
        }

        public static EnabledInput[] GetInputOfType(IEnumerable<EAdventurerAgentChoices> enabledChoices)
        {
            var outputs = GetInputOfType(false);
            foreach (var c in enabledChoices)
            {
                outputs[(int) c].Enabled = true;
            }

            return outputs;
        }
    }
}
