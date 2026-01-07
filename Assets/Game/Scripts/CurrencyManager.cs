namespace EditYourNameSpace
{
    public class CurrencyManager
    {
        GameManager gameManager;
        UserData userData;

        public void Init(GameManager inGameManager)
        {
            gameManager = inGameManager;
            userData = gameManager.userData;
        }

        public bool IsCoinSufficient(int amount)
        {
            bool result = userData.coin >= amount;
            return result;
        }

        public void AddCoin(int amount)
        {
            userData.coin += amount;
        }

        public void SpendCoin(int amount)
        {
            userData.coin -= amount;
        }
    }
}