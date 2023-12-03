using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents;
using UnityEngine;

public class EnvironmentResetter : MonoBehaviour
{
    public AdventurerSystemBehaviour adventurerSystem;
    public RequestShopSystemBehaviour requestSystem;
    public ShopCraftingSystemBehaviour shopCraftingBehaviour;
    
    public float maxTime;

    private float _currentTime;
    // Start is called before the first frame update
    void Start()
    {
        _currentTime = maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        _currentTime -= Time.deltaTime;
        if (_currentTime <= 0)
        {
            var agents = FindObjectsOfType<Agent>();
            foreach (var a in agents)
            {
                a.EndEpisode();
            }
            var adv = FindObjectsOfType<BaseAdventurerAgent>();
            foreach (var agent in adv)
            {
                adventurerSystem.system.RemoveAgent(agent);
            }
            requestSystem.system.requestSystem.ResetRequestSystem();
            shopCraftingBehaviour.system.shopSubSubSystem.ResetShop();
            adventurerSystem.system.Setup();
            _currentTime = maxTime;
        }
    }
}
