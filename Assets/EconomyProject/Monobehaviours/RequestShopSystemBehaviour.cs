using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using UnityEngine;

namespace EconomyProject.Monobehaviours
{
    public class RequestShopSystemBehaviour : MonoBehaviour
    {
        public RequestShopSystem system;
        
        public void Update()
        {
            system.Update();
        }
    }
}
