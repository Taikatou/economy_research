using System;
using UnityEngine;

namespace TurnBased.Scripts.AI
{
    public enum EnemyAction { Attack, PrepareAttack, Block, Wait, None }
    public class EnemyAI
    {
        private readonly EnemyFighterGroup _enemyFighterUnits;
        
        public EnemyAI(EnemyFighterGroup enemyFighterUnits)
        {
            _enemyFighterUnits = enemyFighterUnits;
        }

        public EnemyAction PreviousAction = EnemyAction.None;
        
        public EnemyAction DecideAction(PlayerFighterGroup playerInstance)
        {
            if (PreviousAction == EnemyAction.PrepareAttack)
            {
                Attack(playerInstance);
                Attack(playerInstance);
            }
            else
            {
                var random = new System.Random();
                PreviousAction = (EnemyAction)random.Next((int)EnemyAction.None);
                switch (PreviousAction)
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
            }
            
            return PreviousAction;
        }

        private void Attack(PlayerFighterGroup playerInstance)
        {
            foreach (var fighter in playerInstance.FighterUnits)
            {
                if (fighter.SecondaryAbilityStatus == SecondaryAbilityStatus.Blocking && !fighter.IsDead)
                {
                    _enemyFighterUnits.Instance.Attack(fighter);
                    return;
                }
            }

            var maxDamage = DamageModifier.Weak;
            PlayerFighterData playerToAttack = null;
            foreach (var fighter in playerInstance.FighterUnits)
            {
                if (!fighter.IsDead)
                {
                    var damage = FightingRelationships.GetDamage(fighter.AdventurerType, _enemyFighterUnits.FighterType);
                    if (damage >= maxDamage)
                    {
                        maxDamage = damage;
                        playerToAttack = fighter;
                    }
                }
            }

            if (playerToAttack != null)
            {
                _enemyFighterUnits.Instance.Attack(playerToAttack);
            }
        }
    }
}
