using System;
using UnityEngine;

namespace TurnBased.Scripts.AI
{
    public enum EnemyAction { Attack, Block, Wait }
    
    public delegate void OnAction();
    
    public class EnemyAI
    {
        private readonly EnemyFighterGroup _enemyFighterUnits;

        public EnemyAction NextAction { get; private set; }

        public EnemyAI(EnemyFighterGroup enemyFighterUnits)
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

        public EnemyAction DecideAction(PlayerFighterGroup playerInstance)
        {
            EnemyAction privateAction = NextAction;
            switch (NextAction)
            {
                case EnemyAction.Attack:
                    Attack(playerInstance);
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

        private void Attack(PlayerFighterGroup playerInstance)
        {
            foreach (var fighter in playerInstance.FighterUnits)
            {
                if (fighter.Blocking)
                {
                    _enemyFighterUnits.Instance.Attack(fighter);
                    return;
                }
            }

            var maxDamage = DamageModifier.Weak;
            PlayerFighterData playerToAttack = null;
            foreach (var fighter in playerInstance.FighterUnits)
            {
                var damage = FightingRelationships.GetDamage(fighter.adventurerType, _enemyFighterUnits.FighterType);
                if (damage >= maxDamage)
                {
                    damage = maxDamage;
                    playerToAttack = fighter;
                }
            }

            if (playerToAttack != null)
            {
                _enemyFighterUnits.Instance.Attack(playerToAttack);
            }
        }
    }
}
