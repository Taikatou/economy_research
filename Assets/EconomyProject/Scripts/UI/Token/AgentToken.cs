using UnityEngine;

namespace EconomyProject.Scripts.UI.Token
{
	public class AgentToken<T, TQ> : MonoBehaviour
	{
		public T agent;
		public TQ previousChoice;

		public Transform agentList;
		public Transform shop;
		public Transform request;

		public void Start()
		{
			// Init the token to the default position 
			this.transform.position = agentList.position;
		}
	}
}
