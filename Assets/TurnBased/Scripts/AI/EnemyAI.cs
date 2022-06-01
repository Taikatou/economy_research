using System;
using UnityEngine;

namespace TurnBased.Scripts.AI
{
    public enum EnemyAction { Attack, Block, Wait }
    
    public delegate void OnAction();
    
    public class EnemyAI
    {
        private readonly FighterGroup _enemyFighterUnits;

        public EnemyAction NextAction { get; private set; }

        public EnemyAI(FighterGroup enemyFighterUnits)
        {
            _enemyFighterUnits = enemyFighterUnits;
            
        }

        private void ChooseRandomAction()
        {
            var random = new System.Random();
            var values = Enum.GetValues(typeof(EnemyAction));
            var randomAction = (EnemyAction)values.GetValue(random.Next(values.Length));
            NextAction = randomAction;
        }

        public EnemyAction DecideAction(BaseFighterData playerInstance)
        {
            EnemyAction privateAction = NextAction;
            switch (NextAction)
            {
                case EnemyAction.Attack:
                    _enemyFighterUnits.Instance.Attack(playerInstance);
                    break;
                case EnemyAction.Block:
                    _enemyFighterUnits.Instance.Block();
                    break;
                case EnemyAction.Wait:
                    _enemyFighterUnits.Instance.Wait();
                    break;
            }
            ChooseRandomAction();
            return privateAction;
        }
    }
}
