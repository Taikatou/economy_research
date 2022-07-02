using System;
using UnityEngine;

namespace Data
{
    public interface ILastUpdate
    {
        DateTime LastUpdated { get; }
        void Refresh();
    }

    public class LastUpdate : MonoBehaviour, ILastUpdate
    {
        public DateTime LastUpdated { get; private set; }
        public virtual void Refresh()
        {
            LastUpdated = DateTime.Now;
        }
    }
}