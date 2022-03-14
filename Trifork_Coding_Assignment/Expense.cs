
namespace Trifork_Coding_Assignment
{
    public class Expense
    {
        public int id;
        public string? name;
        public int paidByUserId;
        public double price;
        public DateTime date;
        
        private UserPayer[] users;
        private Group group;

        public Expense(Group group, UserAccount user, string name, double price)
        {
            this.id = 0;
            this.paidByUserId = user.id;
            this.name = name;
            this.price = price;
            this.date = DateTime.Now;
            this.group = group;

            if (group.users.Count > 1)
            {
                users = new UserPayer[group.users.Count];

                for (int i = 0; i < group.users.Count; i++)
                {
                    UserAccount user_ = group.users.ElementAt(i).Value;
                    double amount = price / group.users.Count;
                    users[i] = new UserPayer(user_, amount);
                    users[i].hasPaid = user_.id == this.paidByUserId;
                }
            }
            else
            {
                users = new UserPayer[0];
            }
        }

        /// <summary>
        ///  A user pay its share to the user who has paid the expense 
        /// </summary>
        /// <param name="user"></param>
        public void userPay(UserAccount user)
        {
            UserPayer paidUser = Array.Find(users, user_ => user_.user.id == user.id && user.id != this.paidByUserId);

            if (paidUser != null && !paidUser.hasPaid)
            {
                UserAccount paidByUser = group.users[this.paidByUserId];
                if (paidByUser != null)
                {
                    user.TransfereMoneyTo(paidByUser, paidUser.amount);
                    paidUser.hasPaid = true;
                }
            }
        }

        /// <summary>
        /// Get the amount the user own of the expense
        /// </summary>
        /// <param name="user"></param>
        /// 
        /// <returns> The amount of money</returns>
        public double getAmount(UserAccount user)
        {
            if(user == null) { return 0; }

            UserPayer paidUser = Array.Find(users, user_ => user_.user.id == user.id && user.id != this.paidByUserId);

            if(paidUser != null)
            {
                return paidUser.amount;
            } else
            {
                return 0;
            }
        }

        /// <summary>
        ///  Check if the user has paid 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool hasUserPaid(UserAccount user)
        {
            if (user == null)
            {
                return false;
            }

            UserPayer paidUser = Array.Find(users, user_ => user_.user.id == user.id);

            return paidUser != null && paidUser.hasPaid;
        }

        /// <summary>
        ///  Mark the expense be paid by the user
        /// </summary>
        /// <param name="userId"></param>
        internal void markAsPaid(int userId)
        {
            UserPayer paidUser = Array.Find(users, user_ => user_.user.id == userId && userId != this.paidByUserId);

            if(paidUser != null)
            {
                paidUser.hasPaid = true;
            }
         
        }

        /// <summary>
        ///     Check all user has paid there share to the owner 
        /// </summary>
        /// <returns></returns>
        public bool isCompleted()
        {
            bool finish = true;
        
            foreach (UserPayer paidUser in users )
            {
                finish = paidUser.hasPaid;

                if (!finish)
                {
                    break;
                }
            }

            return finish;
        }

        public override string ToString()
        {
            return String.Format("id: {0}, paidBy: {1}, price: {2}, has Been Paid: {3}", id, paidByUserId, price, isCompleted());
        }
    }

    public class UserPayer
    {
        public UserAccount user;
        public bool hasPaid = false;
        public double amount;

        public UserPayer(UserAccount user, double amount)
        {
            this.user = user;
            this.amount = amount;
        }

        public override string ToString()
        {
            return String.Format("id: {0}, hasPaid: {1}, amount: {2}", user.id, hasPaid, amount);
        }
    }

}
