using System;
using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy
{
    public static class EconomySystemUtils<T> where T : Enum
    {
        private static EnabledInput[] GetInputOfType(bool enabled)
        {
            var actions = Enum.GetValues(typeof(T));
            var output = new EnabledInput[actions.Length];
            foreach (var action in actions)
            {
                var i = (int) action;
                output[i] = new EnabledInput { Input = i, Enabled = enabled };
            }
            return output;
        }

        public static EnabledInput[] GetInputOfType(IEnumerable<T> enabledChoices, int branch)
        {
            var outputs = GetInputOfType(false);
            foreach (var c in enabledChoices)
            {
                var i = Convert.ToInt32(c);
                outputs[i].Enabled = true;
                outputs[i].Branch = branch;
            }

            return outputs;
        }
    }
}
