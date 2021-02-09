using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Config
{
	public abstract class AbstractList<T, TQ> : MonoBehaviour
	{
		//Prefab with the informations and buttons
		public GameObject item;
		//Where the items are stored
		public GameObject holder;
		
		//List or dictionnary with all the parameter values
		protected T _items;

		void Start()
		{
			SetupItems();
		}

		/// <summary>
		/// Set up _items
		/// </summary>
		public abstract void SetupItems();

		/// <summary>
		/// Set up the list of items
		/// </summary>
		public abstract void SetupList();

		public abstract void SetItem(TQ itemToModify, int newPrice);

		/// <summary>
		/// Return the parameters used in ConfigSystem.cs
		/// </summary>
		public T GetParameters()
		{
			return _items;
		}


	}
}
