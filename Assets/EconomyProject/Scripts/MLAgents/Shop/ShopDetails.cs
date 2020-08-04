namespace EconomyProject.Scripts.MLAgents.Shop
{
    [System.Serializable]
    public struct ShopDetails
    {
        public int price;
        public int stock;

        public bool DeductStock(int number)
        {
            var newValue = stock - number;
            stock = newValue < 0 ? 0 : newValue;
            return newValue == 0;
        }

        public void AddStock(int number)
        {
            if (number > 0)
            {
                stock += number;
            }
        }

        public void SetPrice(int increase)
        {
            if (increase > 0)
            {
                price += increase; 
            }
        }
    }
}
