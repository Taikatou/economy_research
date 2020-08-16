using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBased.Scripts.UI
{
    public class BattleUi : MonoBehaviour
    {
        public AdventurerSystem adventurerSystem;
        public GetCurrentAdventurerAgent currentAgent;

        public GameObject characterPrefab;
        
        public Transform playerBattleStation;
        public Transform enemyBattleStation;
        
        public Text dialogueText;

        public BattleHud playerHud;
        public BattleHud enemyHud;
        
        private BattleSubSystem BattleSubSystem => adventurerSystem.GetSubSystem(currentAgent.CurrentAgent);
        
        private BattleSubSystem _cachedSubSystem;
        
        public void SetupBattle(FighterData playerUnit, FighterData enemyUnit)
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
            if (BattleSubSystem != null)
            {
                dialogueText.text = BattleSubSystem.DialogueText;
                if (_cachedSubSystem != BattleSubSystem)
                {
                    _cachedSubSystem = BattleSubSystem;
                    SetupBattle(BattleSubSystem.PlayerFighterUnit, BattleSubSystem.EnemyFighterUnit);
                }
            }
        }
    }
}
