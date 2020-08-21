using UnityEngine;

namespace TurnBased.Scripts.UI
{
    public class CharacterUi : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;

        public void UpdateCharacter(BaseFighterData fighterUnit, bool flipped)
        {
            spriteRenderer.sprite = fighterUnit.Sprite;
            spriteRenderer.flipX = flipped;
        }
    }
}
