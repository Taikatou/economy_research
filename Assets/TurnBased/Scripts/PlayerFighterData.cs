﻿using System.Collections.Generic;
using Data;
using Inventory;
using TurnBased.Scripts.AI;


namespace TurnBased.Scripts
{
    public delegate void OnAfterAttack();
    public delegate UsableItem GetUsableItem();
    public delegate int BonusDamage();
    public class PlayerFighterData : BaseFighterData
    {
        public int level = 0;
        public EAdventurerTypes AdventurerType { get; }

        public OnAfterAttack OnAfterAttack { get; set; }
        public GetUsableItem GetUsableItem { get; set; }
        public BonusDamage BonusDamage { get; set; }

        protected override double Accuracy => 1;
        public override float BlockReduction => 2.0f;
        public override int Level => level;

        public Dictionary<EBattleAction, AttackAction> AttackActionMap =>
            PlayerActionMap.GetAttackActionMap(AdventurerType);

        private UsableItem UsableItem => GetUsableItem.Invoke();

        public override int Damage
        {
            get
            {
                var damage = UsableItem.itemDetails.damage;
                if (BonusDamage != null)
                {
                    damage += BonusDamage.Invoke();
                }
                return damage;
            }
        }

        public PlayerFighterData(EAdventurerTypes adventurerType)
        {
            AdventurerType = adventurerType;
            UnitName = adventurerType.ToString();
        }

        protected override void AfterAttack()
        {
            OnAfterAttack?.Invoke();
        }
        
        public void ResetHp()
        {
            CurrentHp = MaxHp;
        }

        public AttackAction GetAttackAction(EBattleAction action)
        {
            if (AttackActionMap.ContainsKey(action))
            {
                return AttackActionMap[action];
            }

            return null;
        }
    }
}
