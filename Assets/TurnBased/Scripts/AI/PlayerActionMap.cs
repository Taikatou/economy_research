using System.Collections.Generic;
using System.Linq;
using Data;

namespace TurnBased.Scripts.AI
{
    public enum AttackOptions {Attack, Block, Parry, Evade, Heal, None}
    public static class PlayerActionMap
    {
        private static readonly Dictionary<AttackAction, AttackOptions> GetAttackString =
            new()
            {
                { AttackDelegate, AttackOptions.Attack },
                { BlockDelegate, AttackOptions.Block },
                { ParryDelegate, AttackOptions.Parry },
                { EvadeDelegate, AttackOptions.Evade },
                { HealDelegate, AttackOptions.Heal }
            };

        public static AttackOptions GetAttack(AttackAction action)
        {
            if (action != null)
            {
                if (GetAttackString.ContainsKey(action))
                {
                    return GetAttackString[action];
                }   
            }
            return AttackOptions.None;
        }
        
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

        private static readonly Dictionary<AttackAction, int> SwordsmanLevelStructure = new()
        {
            {AttackDelegate, 0},
            {ParryDelegate, 0}
        };

        
        private static readonly Dictionary<AttackAction, int> BrawlerLevelStructure = new()
        {
            {AttackDelegate, 0},
            {BlockDelegate, 0},
            {HealDelegate, 0}
        };
        
        private static readonly Dictionary<AttackAction, int> MageLevelStructure = new()
        {
            {AttackDelegate, 0},
            {EvadeDelegate, 0},
            {HealDelegate, 0}
        };

        private static readonly Dictionary<EAdventurerTypes, Dictionary<AttackAction, int>> Moves = new()
        {
            {EAdventurerTypes.Brawler, BrawlerLevelStructure},
            {EAdventurerTypes.Swordsman, SwordsmanLevelStructure},
            {EAdventurerTypes.Mage, MageLevelStructure}
        };

        private static List<AttackAction> GetAbilitiesUtil(int level, Dictionary<AttackAction, int> levelStructure)
        {
            var abilities = new List<AttackAction>(); 
            foreach(var move in levelStructure)
            {
                if (move.Value < level)
                {
                    abilities.Add(move.Key);
                }
            }
            
            return abilities;
        }

        public static List<AttackAction> GetAbilities(EAdventurerTypes adventurerType, int level)
        {
            List<AttackAction> toReturn = null;
            if (Moves.ContainsKey(adventurerType))
            {
                toReturn = GetAbilitiesUtil(level, Moves[adventurerType]);
            }
            return toReturn;
        }
        
        public static Dictionary<EBattleAction, AttackAction> GetAttackActionMap(EAdventurerTypes adventurerType, List<AttackAction> attackActions)
        {
            var moveDictionary = new Dictionary<EBattleAction, AttackAction>();
            for (var i = 0; i < attackActions.Count(); i++)
            {
                var action = (EBattleAction) i;
                moveDictionary.Add(action, attackActions[i]);
            }
            return moveDictionary;
        }
    }
}
