﻿using System;
using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.DataLoggers;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class CraftingRequestRecord : MonoBehaviour
    {
        public RequestDataLogger rDataLogger;
        
        private Dictionary<RequestTaker, List<CraftingResourceRequest>> _currentRequests;

        public CraftingResourceRequest[] GetCurrentRequests(RequestTaker requestTaker)
        {
            var toReturn = new List<CraftingResourceRequest>();
            if (_currentRequests.ContainsKey(requestTaker))
            {
                var requests  = _currentRequests[requestTaker];
                toReturn.AddRange(requests);
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


		public void TakeRequest(RequestTaker requestTaker, CraftingResourceRequest craftingResourceRequest)
        {
            var contains = _currentRequests.ContainsKey(requestTaker);
            if (!contains)
            {
                _currentRequests[requestTaker] = new List<CraftingResourceRequest>();
            }
            _currentRequests[requestTaker].Add(craftingResourceRequest);
            craftingResourceRequest.TakenTime = Time.time;
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

                    request.CompletedTime = Time.time;
                    rDataLogger.AddRequestCompleted(request);
                }
            }
        }
    }
}
