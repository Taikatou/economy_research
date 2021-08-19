using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public abstract class RequestTaker : MonoBehaviour
    {
        public abstract IEnumerable<CraftingResourceRequest> GetItemList();

        public abstract void TakeRequest(CraftingResourceRequest request);

        public abstract void CompleteRequest(int reward);
    }
}
