using System;

namespace LevelSystem
{
    public class MediumFastLevelUpComponent : LevelUpComponent
    {
        public override int Level => (int) Math.Floor(Math.Sqrt(TotalExp));
        protected override void LevelUpCheck()
        {
            throw new NotImplementedException();
        }
    }
}
