using System;
using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy
{
    public static class EconomySystemUtils<T> where T : Enum
    {
        private static EnabledInput[] GetInputOfType<T>(bool enabled)
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

        public static EnabledInput[] GetInputOfType(IEnumerable<T> enabledChoices)
        {
            var outputs = GetInputOfType<T>(false);
            foreach (var c in enabledChoices)
            {
                var i = Convert.ToInt32(c);
                outputs[i].Enabled = true;
            }

            return outputs;
        }
    }
}
