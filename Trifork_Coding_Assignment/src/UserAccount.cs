namespace Trifork_Coding_Assignment
{
    public class UserAccount
    {
        public int id;
        public String? userName = null;
        public decimal money = 0.0M;

        public UserAccount (int id, String name, decimal money)
        {
            this.id = id;
            this.money = money;
            this.userName = name;
        }

     
        /// <summary>
        ///  Transfere the money from this user to the user 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="amount"></param>
        public void TransfereMoneyTo(UserAccount user, decimal amount)
        {
            this.money -= amount;
            user.money += amount;
        }

        /// <summary>
        ///  User has paid for an expense
        /// </summary>
        /// <param name="amount"></param>
        public void pay(decimal amount)
        {
            this.money -= amount;
        }

        public override String ToString()
        {
            return String.Format("id: {0}, name: {1}, money left: {2}", id, userName, money);
        }

    }
}
