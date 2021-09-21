using EconomyProject.Scripts.MLAgents;
using EconomyProject.Scripts.UI;
using UnityEngine;

public class GetCurrentAgentAggregator : MonoBehaviour
{
    public GetCurrentAdventurerAgent CurrentAdventurerAgent;
    public GetCurrentShopAgent CurrentShopAgent;

    public ToggleButton ToggleButton;

    public IEconomyAgent CurrentAgent => ToggleButton.craftActive ? (IEconomyAgent)CurrentShopAgent.CurrentAgent : 
                                                        (IEconomyAgent)CurrentAdventurerAgent.CurrentAgent;
}
