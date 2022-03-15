
namespace Trifork_Coding_Assignment
{
    public class Expense
    {
        public string? name;
        public int paidByUserId;
        public decimal price;
        public DateTime date;
        
        private UserPayer[] users;
        private Group group;

        public Expense(Group group, UserAccount user, string name, decimal price)
        {
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
                    users[i] = new UserPayer(user_);
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
                    user.TransfereMoneyTo(paidByUser, 0);
                    paidUser.hasPaid = true;
                }
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

            UserPayer paidUser = Array.Find(users, user_ => user_ != null && user_.user.id == user.id);

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
            UserAccount user = group.users[paidByUserId];
            return String.Format("Expense: paid by: (id: {0}, name: {1}), costed: {2}, has Been Paid: {3}", user.id, user.userName, price, isCompleted());
        }
    }

    public class UserPayer
    {
        public UserAccount user;
        public bool hasPaid = false;

        public UserPayer(UserAccount user)
        {
            this.user = user;
        }

        public override string ToString()
        {
            return String.Format("id: {0}, hasPaid: {1}", user?.id, this.hasPaid);
        }
    }

}
