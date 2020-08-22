using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class CraftingRequestRecord : MonoBehaviour
    {
        private readonly Dictionary<RequestTaker, List<CraftingResourceRequest>> _currentRequests;

        public List<CraftingResourceRequest> GetCurrentRequests(RequestTaker requestTaker)
        {
            if (_currentRequests.ContainsKey(requestTaker))
            {
                return _currentRequests[requestTaker];
            }
            return new List<CraftingResourceRequest>();
        }

        public CraftingRequestRecord()
        {
            _currentRequests = new Dictionary<RequestTaker, List<CraftingResourceRequest>>();
        }

        public void AddRequest(RequestTaker requestTaker, CraftingResourceRequest craftingResourceRequest)
        {
            var contains = _currentRequests.ContainsKey(requestTaker);
            if (!contains)
            {
                _currentRequests[requestTaker] = new List<CraftingResourceRequest>();
            }
            _currentRequests[requestTaker].Add(craftingResourceRequest); 
        }

        public void CompleteRequest(RequestTaker taker, CraftingResourceRequest request)
        {
            if (_currentRequests.ContainsKey(taker))
            {
                if (_currentRequests[taker].Contains(request))
                {
                    taker.CompleteRequest(request.Reward);
                    request.TransferResource();

                    _currentRequests[taker].Remove(request);

                    var count = _currentRequests[taker].Count;
                    if (count == 0)
                    {
                        _currentRequests.Remove(taker);
                    }
                }
            }
        }
    }
}
