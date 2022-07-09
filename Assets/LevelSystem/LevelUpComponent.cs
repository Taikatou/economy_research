using UnityEngine;

namespace LevelSystem
{
    public abstract class LevelUpComponent : MonoBehaviour
    {
        public float TotalExp { get; private set; }

        public abstract int Level { get; }

        public virtual void Start()
        {
            TotalExp = 0;
        }

        public void AddExp(float exp)
        {
            Debug.Log(exp);
            if (exp > 0)
            {
                TotalExp += exp;
                LevelUpCheck();
            }
        }

        public void Reset()
        {
            TotalExp = 0;
            LevelUpCheck();
        }

        protected abstract void LevelUpCheck();
    }
}
