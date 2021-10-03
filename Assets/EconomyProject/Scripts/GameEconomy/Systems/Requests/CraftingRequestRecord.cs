using System;
using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class CraftingRequestRecord : MonoBehaviour
    {
        private Dictionary<RequestTaker, List<CraftingResourceRequest>> _currentRequests;

        public CraftingResourceRequest[] GetCurrentRequests(RequestTaker requestTaker, int limit=0)
        {
            var toReturn = new List<CraftingResourceRequest>();
            limit = limit == 0 ? Int32.MaxValue : limit;
            if (_currentRequests.ContainsKey(requestTaker))
            {
                var requests  = _currentRequests[requestTaker];
                for (var i = 0; i < limit && i < requests.Count; i++)
                {
                    toReturn.Add(requests[i]);
                }
            }

            return toReturn.ToArray();
        }

        public CraftingRequestRecord()
        {
			ResetCraftingRequestRecord();
		}

		public void ResetCraftingRequestRecord()
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
