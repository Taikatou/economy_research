using System;
using UnityEngine;

namespace TurnBased.Scripts.UI
{
    public class CharacterUi : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;

        public void UpdateCharacter<T, Q>(BaseFighterData<T, Q> fighterUnit, bool flipped) where T : Enum where Q : Enum
        {
            spriteRenderer.sprite = fighterUnit.Sprite;
            spriteRenderer.flipX = flipped;
        }
    }
}
