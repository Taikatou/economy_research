using System;
using UnityEngine;

namespace LevelSystem
{
    public abstract class LevelUpComponent : MonoBehaviour
    {
        public float MaxExp;
        protected float TotalExp { get; set; }

        public abstract int Level { get; }

        public void Start()
        {
            TotalExp = 0;
        }

        public void AddExp(float exp)
        {
            if (exp > 0)
            {
                TotalExp = Math.Max(TotalExp + exp, MaxExp);
            }
        }
    }
}
