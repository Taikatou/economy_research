using EconomyProject.Scripts.GameEconomy.Systems;
using UnityEngine;

namespace EconomyProject.Monobehaviours
{
    public class AdventurerSystemBehaviour : MonoBehaviour
    {
        public AdventurerSystem system;
        public void FixedUpdate()
        {
            system.FixedUpdate();
        }
    }
}
