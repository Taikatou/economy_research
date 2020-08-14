using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBased
{
	public enum BattleState { Start, PlayerTurn, EnemyTurn, Won, Lost }

	public class BattleSystem : MonoBehaviour
	{
		public float waitFor = 2;
		public GameObject playerPrefab;
		public GameObject enemyPrefab;

		public Transform playerBattleStation;
		public Transform enemyBattleStation;

		private FighterUnit _playerFighterUnit;
		private FighterUnit _enemyFighterUnit;

		public Text dialogueText;

		public BattleHud playerHud;
		public BattleHud enemyHud;

		public BattleState state;

		// Start is called before the first frame update
		private void Start()
		{
			state = BattleState.Start;
			StartCoroutine(SetupBattle());
		}

		private IEnumerator SetupBattle()
		{
			var playerGo = Instantiate(playerPrefab, playerBattleStation);
			_playerFighterUnit = playerGo.GetComponent<FighterUnit>();

			var enemyGo = Instantiate(enemyPrefab, enemyBattleStation);
			_enemyFighterUnit = enemyGo.GetComponent<FighterUnit>();

			dialogueText.text = "A wild " + _enemyFighterUnit.unitName + " approaches...";

			playerHud.SetHud(_playerFighterUnit);
			enemyHud.SetHud(_enemyFighterUnit);

			yield return new WaitForSeconds(waitFor);

			state = BattleState.PlayerTurn;
			PlayerTurn();
		}

		private IEnumerator PlayerAttack()
		{
			var isDead = _enemyFighterUnit.TakeDamage(_playerFighterUnit.damage);

			enemyHud.SetHp(_enemyFighterUnit.currentHp);
			dialogueText.text = "The attack is successful!";

			yield return new WaitForSeconds(2f);

			if(isDead)
			{
				state = BattleState.Won;
				EndBattle();
			} else
			{
				state = BattleState.EnemyTurn;
				StartCoroutine(EnemyTurn());
			}
		}

		private IEnumerator EnemyTurn()
		{
			dialogueText.text = _enemyFighterUnit.unitName + " attacks!";

			yield return new WaitForSeconds(1f);

			var isDead = _playerFighterUnit.TakeDamage(_enemyFighterUnit.damage);

			playerHud.SetHp(_playerFighterUnit.currentHp);

			yield return new WaitForSeconds(1f);

			if(isDead)
			{
				state = BattleState.Lost;
				EndBattle();
			} else
			{
				state = BattleState.PlayerTurn;
				PlayerTurn();
			}
		}

		private void EndBattle()
		{
			if(state == BattleState.Won)
			{
				dialogueText.text = "You won the battle!";
			} else if (state == BattleState.Lost)
			{
				dialogueText.text = "You were defeated.";
			}
		}

		private void PlayerTurn()
		{
			dialogueText.text = "Choose an action:";
		}

		private IEnumerator PlayerHeal()
		{
			_playerFighterUnit.Heal(5);

			playerHud.SetHp(_playerFighterUnit.currentHp);
			dialogueText.text = "You feel renewed strength!";

			yield return new WaitForSeconds(2f);

			state = BattleState.EnemyTurn;
			StartCoroutine(EnemyTurn());
		}

		public void OnAttackButton()
		{
			if (state != BattleState.PlayerTurn)
				return;

			StartCoroutine(PlayerAttack());
		}

		public void OnHealButton()
		{
			if (state != BattleState.PlayerTurn)
				return;

			StartCoroutine(PlayerHeal());
		}
	}
}