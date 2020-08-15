using UnityEngine;

namespace TurnBased.Scripts.UI
{
    public class CharacterUi : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;

        public void UpdateCharacter(FighterUnit fighterUnit)
        {
            spriteRenderer.sprite = fighterUnit.sprite;
        }
    }
}
