using System.Collections.Generic;
using Data;

namespace TurnBased.Scripts
{
    public enum DamageModifier {Weak, Neutral, Strong};
    public enum EFighterType { Normal, Poisin, Flying, Rock }

    public static class FightingRelationships
    {
        private static Dictionary<EAdventurerTypes, Dictionary<EFighterType, DamageModifier>> Relationships => new Dictionary<EAdventurerTypes, Dictionary<EFighterType, DamageModifier>>
        {
            {
                EAdventurerTypes.Brawler, new Dictionary<EFighterType, DamageModifier>
                {
                    {EFighterType.Rock, DamageModifier.Strong},
                    {EFighterType.Flying, DamageModifier.Weak}
                }
            },
            {
                EAdventurerTypes.Swordsman, new Dictionary<EFighterType, DamageModifier>
                {
                    {EFighterType.Poisin, DamageModifier.Strong},
                    {EFighterType.Rock, DamageModifier.Weak}
                }
            },
            {
                EAdventurerTypes.Mage, new Dictionary<EFighterType, DamageModifier>
                {
                    {EFighterType.Flying, DamageModifier.Strong},
                    {EFighterType.Poisin, DamageModifier.Weak}
                }
            }
        };

        public static DamageModifier GetDamage(EAdventurerTypes adventurerTypes, EFighterType fighterType)
        {
            var toReturn = DamageModifier.Neutral;
            if (Relationships.ContainsKey(adventurerTypes))
            {
                if (Relationships[adventurerTypes].ContainsKey(fighterType))
                {
                    toReturn = Relationships[adventurerTypes][fighterType];
                }
            }

            return toReturn;
        }

        public static DamageModifier GetDamage(EFighterType fighterType, EAdventurerTypes adventurerTypes)
        {
            var map = new Dictionary<DamageModifier, DamageModifier>
            {
                {DamageModifier.Strong, DamageModifier.Weak},
                {DamageModifier.Neutral, DamageModifier.Neutral},
                {DamageModifier.Weak, DamageModifier.Strong}
            };
            return map[GetDamage(adventurerTypes, fighterType)];
        }
    }
}
