using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public abstract class RequestTaker : MonoBehaviour
    {
        public abstract List<ResourceRequest> ItemList { get; }

        public abstract void TakeRequest(ResourceRequest request);

        public virtual void CompleteRequest()
        {

        }
    }
}
