using System.Collections.Generic;
using Data;

namespace TurnBased.Scripts.AI
{
    public static class PlayerActionMap
    {
        private static string AttackDelegate(EnemyFighterGroup enemyFighterUnits, PlayerFighterData instance)
        {
            instance.Attack(enemyFighterUnits.Instance);

            return "The attack is successful!";
        }
        
        private static string BlockDelegate(EnemyFighterGroup enemyFighterUnits, PlayerFighterData instance)
        {
            instance.Block();
			
            return "You are preparing for an attack";
        }
        
        private static string ParryDelegate(EnemyFighterGroup enemyFighterUnits, PlayerFighterData instance)
        {
            instance.Parry();
			
            return "You are ready to parry";
        }
        
        private static string EvadeDelegate(EnemyFighterGroup enemyFighterUnits, PlayerFighterData instance)
        {
            instance.Evade();
			
            return "You are ready to parry";
        }
        
        private static string HealDelegate(EnemyFighterGroup enemyFighterUnits, PlayerFighterData instance)
        {
            instance.Heal(5);
			
            return "You feel renewed strength!";
        }
        
        public static Dictionary<EBattleAction, AttackAction> GetAttackActionMap(EAdventurerTypes adventurerType)
        {
            var brawlerDictionary = new Dictionary<EBattleAction, AttackAction>
            {
                { EBattleAction.PrimaryAction, AttackDelegate },
                { EBattleAction.SecondaryAction, BlockDelegate },
                { EBattleAction.BonusAction, HealDelegate }
            };
            
            var swordsManDictionary = new Dictionary<EBattleAction, AttackAction>
            {
                { EBattleAction.PrimaryAction, AttackDelegate },
                { EBattleAction.SecondaryAction, ParryDelegate }
            };
            
            var mageDictionary = new Dictionary<EBattleAction, AttackAction>
            {
                { EBattleAction.PrimaryAction, AttackDelegate },
                { EBattleAction.SecondaryAction, EvadeDelegate },
                { EBattleAction.BonusAction, HealDelegate }
            };
            var agentChoices = new Dictionary<EAdventurerTypes, Dictionary<EBattleAction, AttackAction>>
            {
                { EAdventurerTypes.Brawler, brawlerDictionary },
                { EAdventurerTypes.Mage, brawlerDictionary },
                { EAdventurerTypes.Swordsman, swordsManDictionary }
            };
            return agentChoices[adventurerType];
        }
    }
}
