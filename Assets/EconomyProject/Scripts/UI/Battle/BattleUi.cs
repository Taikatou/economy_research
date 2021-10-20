using System.Collections.Generic;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts;
using TurnBased.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Battle
{
    public class BattleUi : MonoBehaviour
    {
        public AdventurerSystemBehaviour adventurerSystem;
        public GetCurrentAdventurerAgent currentAgent;

        public GameObject characterPrefab;
        
        public BattleStation playerBattleStation;
        public BattleStation enemyBattleStation;
        
        public Text dialogueText;

        public BattleHud playerHud;
        public BattleHud enemyHud;
        
        private BattleSubSystemInstance<AdventurerAgent> BattleSubSystemInstance => 
            adventurerSystem.system.battleSubSystem.GetSubSystem(currentAgent.CurrentAgent);
        
        private BattleSubSystemInstance<AdventurerAgent> _cachedSubSystemInstance;

        private List<GameObject> _uiGameObjects;

        public void Start()
        {
            _uiGameObjects = new List<GameObject>();
        }

        private void SpawnCharacter(BaseFighterData fighterData, BattleStation station)
        {
            var characterGo = Instantiate(characterPrefab, station.transform);
            var playerCharacterUi = characterGo.GetComponent<CharacterUi>();
            playerCharacterUi.UpdateCharacter(fighterData, station.flipped);
            _uiGameObjects.Add(characterGo);
        }

        private void ClearGameObjects()
        {
            foreach (var item in _uiGameObjects)
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
            if (BattleSubSystemInstance != null)
            {
                dialogueText.text = BattleSubSystemInstance.DialogueText;
                if (_cachedSubSystemInstance != BattleSubSystemInstance)
                {
                    _cachedSubSystemInstance = BattleSubSystemInstance;
                    SetupBattle(BattleSubSystemInstance.PlayerFighterUnits.Instance,
                        BattleSubSystemInstance.EnemyFighterUnits.Instance);
                }
            }
        }
    }
}
