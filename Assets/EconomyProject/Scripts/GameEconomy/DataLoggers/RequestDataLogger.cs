using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;

namespace EconomyProject.Scripts.GameEconomy.DataLoggers
{
    public struct ResourceRequestData
    {
        public CraftingResourceRequest Request;
    }

    public class RequestDataLogger : DataLogger
    {
        private List<ResourceRequestData> _resourceRequests;

        protected override void Start()
        {
            base.Start();
            _resourceRequests = new List<ResourceRequestData>();
        }

        void OnApplicationQuit()
        {
            PrintResourceRequests();
        }

        public void AddRequestCompleted(CraftingResourceRequest resourceRequest)
        {
            _resourceRequests.Add(new ResourceRequestData { Request = resourceRequest});
        }

        private void PrintResourceRequests()
        {
            var rowData = new List<string[]>
            {
                new[] { "Resource", "CompletedTime", "CreationTme", "TakenTime", "Number", "Price" }
            };
            foreach (var item in _resourceRequests)
            {
                var row = new[] {
                    item.Request.Resource.ToString(),
                    item.Request.CompletedTime.ToString(),
                    item.Request.CreationTime.ToString(),
                    item.Request.TakenTime.ToString(),
                    item.Request.Number.ToString(),
                    item.Request.Price.ToString()
                };
                rowData.Add(row);
            }
            OutputCsv(rowData, "request_data");
        }
    }
}
