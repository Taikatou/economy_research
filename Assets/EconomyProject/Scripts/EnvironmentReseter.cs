using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts
{
    public class EnvironmentReseter : MonoBehaviour
    {
        public AdventurerSystemBehaviour adventurerSystem;
        public RequestShopSystemBehaviour requestSystem;
        public ShopCraftingSystemBehaviour shopCraftingBehaviour;
        public Text TextGameObject;
        public float maxTime;
        public static int Reset { get; private set; }
        public static float CurrentTime { get; private set; }
        // Start is called before the first frame update
        void Start()
        {
            Application.targetFrameRate = -1;
            CurrentTime = maxTime;
        }

        // Update is called once per frame
        void Update()
        {
            CurrentTime -= Time.deltaTime;
            if (CurrentTime <= 0)
            {
                Reset++;
                TextGameObject.text = Reset.ToString("n2");
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
                CurrentTime = maxTime;
            
                var locationSelect = FindObjectsOfType<Resetable>();
                foreach (var l in locationSelect)
                {
                    l.Reset();
                }
            }
        }
    }
}
