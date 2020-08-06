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
            stock += number;
        }

        public void SetPrice(int newPrice)
        {
            price = newPrice;
        }
    }
}
