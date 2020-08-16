using UnityEngine;

namespace TurnBased.Scripts.UI
{
    public class CharacterUi : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;

        public void UpdateCharacter(FighterData fighterUnit)
        {
            spriteRenderer.sprite = fighterUnit.sprite;
        }
    }
}
