using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public abstract class RequestTaker : MonoBehaviour
    {
        public abstract List<CraftingResourceRequest> ItemList { get; }

        public abstract void TakeRequest(CraftingResourceRequest request);

        public virtual void CompleteRequest()
        {

        }
    }
}
