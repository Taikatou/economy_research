using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Config
{
	public abstract class AbstractConfigComponent<T, TQ> : MonoBehaviour
	{
		public AbstractList<T,TQ> abstractList;
		public TQ item;
		public int price;

		public Text nameLabel;
		public Text priceText;
		public InputField inputTextPrice;

		public abstract void SetupAbstractList();

		private void Start()
		{
			SetupAbstractList();
		}

		/// <summary>
		/// Setup the componentUI and values
		/// </summary>
		public void Setup(TQ newItem, int newPrice)
		{
			item = newItem;
			price = newPrice;

			nameLabel.text = newItem.ToString();
			priceText.text = newPrice.ToString();
		}

		/// <summary>
		/// Increment price by +1
		/// </summary>
		public void IncreasePrice()
		{
			price++;
			SetPrice();
		}

		/// <summary>
		/// Increment price by -1. Minimum price = 1
		/// </summary>
		public void DecreasePrice()
		{
			if (price <= 1)
			{
				return;
			}

			price--;
			SetPrice();
		}
		/// <summary>
		/// Set price in the inputField and call the ListConfigItems
		/// </summary>
		public void SetPriceInputField()
		{
			if (inputTextPrice.text == null || inputTextPrice.text == "")
			{
				inputTextPrice.text = price.ToString();
				return;
			}
			else
			{
				price = int.Parse(inputTextPrice.text);
			}
			SetPrice();
		}

		/// <summary>
		/// Set the price in the _itemPrices in ListConfigItems.cs
		/// </summary>
		public void SetPrice()
		{
			inputTextPrice.text = price.ToString();
			abstractList.SetItem(item, price);
		}
	}
}