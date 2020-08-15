using UnityEngine;

namespace TurnBased.Scripts
{
    public class BattleActivator : MonoBehaviour
    {
        private bool _cachedActive = true;
        public GameObject toCheck;
        private void Update()
        {
            if (toCheck.activeSelf != _cachedActive)
            {
                _cachedActive = toCheck.activeSelf;
                foreach (Transform child in transform)
                    child.gameObject.SetActive(_cachedActive);
            }
        }
    }
}
