using UnityEngine;

namespace EconomyProject.Scripts.UI
{
    public class ConfigHolder : MonoBehaviour
    {
        public GameObject configUI;
        void Start()
        {
            configUI.transform.position = transform.position;
        }
    }
}
