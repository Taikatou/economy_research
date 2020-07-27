using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class RequestRecord : MonoBehaviour
    {

        private readonly Dictionary<RequestTaker, List<ResourceRequest>> _currentRequests;

        public RequestRecord()
        {
            _currentRequests = new Dictionary<RequestTaker, List<ResourceRequest>>();
        }

        public void AddRequest(RequestTaker requestTaker, ResourceRequest resourceRequest)
        {
            bool contains = _currentRequests.ContainsKey(requestTaker);
            if (!contains)
            {
                _currentRequests[requestTaker] = new List<ResourceRequest>();
            }
            _currentRequests[requestTaker].Add(resourceRequest); 
        }

        public void CompleteRequest(RequestTaker taker, ResourceRequest request)
        {
            if (_currentRequests.ContainsKey(taker))
            {
                if (_currentRequests[taker].Contains(request))
                {
                    taker.CompleteRequest();
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
