using System;
using UnityEngine;

namespace LevelSystem
{
    public abstract class LevelUpComponent : MonoBehaviour
    {
        protected float TotalExp { get; set; }

        public abstract int Level { get; }

        public virtual void Start()
        {
            TotalExp = 0;
        }

        public void AddExp(float exp)
        {
            if (exp > 0)
            {
                TotalExp += exp;
            }
        }
    }
}
