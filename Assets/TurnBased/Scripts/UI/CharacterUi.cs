using UnityEngine;

namespace TurnBased.Scripts.UI
{
    public class CharacterUi : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;

        public void UpdateCharacter(BaseFighterData fighterUnit)
        {
            spriteRenderer.sprite = fighterUnit.Sprite;
        }
    }
}
