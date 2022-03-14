namespace Trifork_Coding_Assignment
{
    public class UserAccount
    {
        public int id;
        public String? userName = null;
        public double money = 0.0;

        public UserAccount (int id, String name, double money)
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
        public void TransfereMoneyTo(UserAccount user, double amount)
        {
            this.money -= amount;
            user.money += amount;
        }

        public override String ToString()
        {
            return String.Format("id: {0}, name: {1}, money left: {2}", id, userName, money);
        }

    }
}
