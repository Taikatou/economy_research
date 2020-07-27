using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class TestRequestTaker : AdventurerRequestTaker
    {
        public float timeToSpawn = 2;

        public RequestSystem requestRecord;

        private float _currentTime;
        private void Update()
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= timeToSpawn)
            {
                _currentTime = 0;
                var allRequests = requestRecord.GetAllCraftingRequests();
                if (allRequests.Count > 0)
                {
                    var random = new System.Random();
                    var itemIndex = random.Next(allRequests.Count);
                    var item = allRequests[itemIndex];

                    requestRecord.TakeRequest(this, item);
                    requestRecord.CompleteRequest(this, item);
                }
            }
        }
    }
}
