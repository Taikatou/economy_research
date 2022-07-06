using System.Collections.Generic;
using System.Linq;
using Data;

namespace TurnBased.Scripts.AI
{
    public enum EAttackOptions {Attack, Block, Parry, Evade, Heal, None}
    public static class PlayerActionMap
    {
        public static readonly Dictionary<EAttackOptions, AttackAction> GetAttackDelegate =
            new()
            {
                { EAttackOptions.Attack, AttackDelegate },
                { EAttackOptions.Block, BlockDelegate },
                { EAttackOptions.Parry, ParryDelegate },
                { EAttackOptions.Evade, EvadeDelegate },
                { EAttackOptions.Heal, HealDelegate }
            };

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

        private static readonly Dictionary<EAttackOptions, int> SwordsmanLevelStructure = new()
        {
            {EAttackOptions.Attack, 0},
            {EAttackOptions.Parry, 0}
        };

        
        private static readonly Dictionary<EAttackOptions, int> BrawlerLevelStructure = new()
        {
            {EAttackOptions.Attack, 0},
            {EAttackOptions.Block, 0},
            {EAttackOptions.Heal, 0}
        };
        
        private static readonly Dictionary<EAttackOptions, int> MageLevelStructure = new()
        {
            {EAttackOptions.Attack, 0},
            {EAttackOptions.Evade, 0},
            {EAttackOptions.Heal, 0}
        };

        private static readonly Dictionary<EAdventurerTypes, Dictionary<EAttackOptions, int>> Moves = new()
        {
            {EAdventurerTypes.Brawler, BrawlerLevelStructure},
            {EAdventurerTypes.Swordsman, SwordsmanLevelStructure},
            {EAdventurerTypes.Mage, MageLevelStructure}
        };

        private static List<EAttackOptions> GetAbilitiesUtil(int level, Dictionary<EAttackOptions, int> levelStructure)
        {
            var abilities = new List<EAttackOptions>(); 
            foreach(var move in levelStructure)
            {
                if (move.Value <= level)
                {
                    abilities.Add(move.Key);
                }
            }
            
            return abilities;
        }

        public static List<EAttackOptions> GetAbilities(EAdventurerTypes adventurerType, int level)
        {
            List<EAttackOptions> toReturn = null;
            if (Moves.ContainsKey(adventurerType))
            {
                toReturn = GetAbilitiesUtil(level, Moves[adventurerType]);
            }
            return toReturn;
        }
        
        public static Dictionary<EBattleAction, EAttackOptions> GetAttackActionMap(EAdventurerTypes adventurerType, List<EAttackOptions> attackActions)
        {
            var moveDictionary = new Dictionary<EBattleAction, EAttackOptions>();
            for (var i = 0; i < attackActions.Count(); i++)
            {
                var action = (EBattleAction) i;
                moveDictionary.Add(action, attackActions[i]);
            }
            return moveDictionary;
        }
    }
}
