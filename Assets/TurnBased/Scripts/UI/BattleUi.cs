using System.Collections.Generic;
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

        public List<GameObject> uiGameObjects;

        public void Start()
        {
            uiGameObjects = new List<GameObject>();
        }

        private void SpawnCharacter(BaseFighterData fighterData, Transform location)
        {
            var characterGo = Instantiate(characterPrefab, location);
            var playerCharacterUi = characterGo.GetComponent<CharacterUi>();
            playerCharacterUi.UpdateCharacter(fighterData);
            uiGameObjects.Add(characterGo);
        }

        private void ClearGameObjects()
        {
            foreach (var item in uiGameObjects)
            {
                Destroy(item);    
            }
        }

        private void SetupBattle(BaseFighterData playerUnit, BaseFighterData enemyUnit)
        {
            ClearGameObjects();

            SpawnCharacter(playerUnit, playerBattleStation);
            SpawnCharacter(enemyUnit, enemyBattleStation);
            
            dialogueText.text = "A wild " + enemyUnit.UnitName + " approaches...";

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
