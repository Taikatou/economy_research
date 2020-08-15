using UnityEngine;
using UnityEngine.UI;

namespace TurnBased.Scripts.UI
{
    public class BattleUi : MonoBehaviour
    {
        public BattleSubSystem battleSubSystem;
        
        public GameObject characterPrefab;
        
        public Transform playerBattleStation;
        public Transform enemyBattleStation;
        
        public Text dialogueText;

        public BattleHud playerHud;
        public BattleHud enemyHud;

        public void SetupBattle(FighterUnit playerUnit, FighterUnit enemyUnit)
        {
            var playerGo = Instantiate(characterPrefab, playerBattleStation);
            var playerCharacterUi = playerGo.GetComponent<CharacterUi>();
            playerCharacterUi.UpdateCharacter(playerUnit);

            var enemyGo = Instantiate(characterPrefab, enemyBattleStation);
            var enemyCharacterUi = enemyGo.GetComponent<CharacterUi>();
            enemyCharacterUi.UpdateCharacter(enemyUnit);
            
            dialogueText.text = "A wild " + enemyUnit.unitName + " approaches...";

            playerHud.SetHud(playerUnit);
            enemyHud.SetHud(enemyUnit);
        }

        private void Update()
        {
            if (battleSubSystem != null)
            {
                dialogueText.text = battleSubSystem.DialogueText;
            }
        }
    }
}
