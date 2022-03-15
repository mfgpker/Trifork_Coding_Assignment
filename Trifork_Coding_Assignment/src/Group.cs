
namespace Trifork_Coding_Assignment
{
    
    public class Group
    {
        public string? groupName = null;
        public Dictionary<int, UserAccount> users = new Dictionary<int, UserAccount>();
        private Dictionary<int, UserBalance> userbalances = new Dictionary<int, UserBalance>();

        private List<Expense> expenses = new List<Expense>();

        public decimal MoneyTotal
        {
            get {
                decimal total = 0;

                foreach (Expense expense in expenses)
                {
                    total += expense.isCompleted() ? 0 : expense.price;
                }

                return total;
            }
        }

        /// <summary>
        ///  Get all unpaid expenses 
        /// </summary>
        public List<Expense> Expenses
        {
            get
            {
                return expenses.FindAll(exp => !exp.isCompleted());
            }
        }

        public Group(String groupName, List<UserAccount> usersList)
        {
            this.groupName = groupName;

            foreach(var user in usersList)
            {
                this.users.Add(user.id, user);
                this.userbalances.Add(user.id, new UserBalance(user));
            }
        }

        /// <summary>
        ///     Add a expense to the group
        /// </summary>
        /// 
        /// <param name="userId"></param>
        /// <param name="price"></param>
        /// <param name="name"></param>
        public void AddExpense(int userId, decimal price, string name)
        {
            users[userId].spent(price);

            expenses.Add(new Expense(this, users[userId], name, price));

        }

        /// <summary>
        ///     Get all expenses the user has created
        /// </summary>
        /// 
        /// <param name="userId"></param>
        /// 
        /// <returns>List of all unpaid expenses</returns>
        public List<Expense> GetAllExpensesMadeByUser(int userId)
        {
            return expenses.FindAll(expense => expense.paidByUserId == userId);
        }

        /// <summary>
        ///     Get the amount money the user has spent
        /// </summary>
        /// 
        /// <param name="userId"></param>
        /// 
        /// <returns>total money the user has paid for</returns>
        public decimal GetUserSpent(int userId)
        {
            decimal total = 0;

            foreach (Expense expense in GetAllExpensesMadeByUser(userId))
            {
                total += expense.isCompleted() ? 0 : expense.price;
            }

            return total;
        }

        /// <summary>
        ///     Get all expenses the user has not paid yet
        /// </summary>
        /// 
        /// <param name="userId"></param>
        /// 
        /// <returns>List of all unpaid expenses</returns>
        public List<Expense> GetAllUnPaidExpenses(int userId)
        {
            if (users[userId] == null)
            {
                return new List<Expense>();
            }

            return expenses.FindAll(expense => !expense.hasUserPaid(users[userId]));
        }

        /// <summary>
        ///  Get a list of payments that would settle all debts 
        /// </summary>
        /// 
        /// <returns>list of payments </returns>
        public List<UserBalance> GetListOfPayments()
        {
            List<UserBalance> balances = new List<UserBalance>();

            foreach (KeyValuePair<int, UserAccount> userKvp in users)
            {
                UserBalance balance = GetUserBalance(userKvp.Value.id);

                balances.Add(balance);
            }

            foreach(UserBalance balance in balances)
            {
                // this user need money from the others
                if(balance.balance > 0)
                {
                    foreach (UserBalance balance2 in balances)
                    {
                        if (balance.balance == 0)
                        {
                            break;
                        }
                        // this user need money from the others
                        if (balance2.user.id != balance.user.id && balance2.balance < 0)
                        {
                            if (Math.Abs(balance2.balance) <= balance.balance)
                            {
                                decimal amount = Math.Abs(balance2.balance);
                                Debt debt = new Debt();
                                debt.amount = amount;
                                balance2.debt.Add(balance.user.id, debt);

                                balance2.balance = 0;
                                balance.balance = Math.Round(balance.balance - amount, 2);
                            } else
                            {
                                decimal amount = Math.Round(Math.Abs(balance2.balance) - balance.balance, 2);
                                Debt debt = new Debt();
                                debt.amount = amount;

                                balance2.debt.Add(balance.user.id, debt);
                                balance2.balance = -amount;
                                balance.balance = Math.Round(balance.balance - amount, 2);
                            }
                        }
                    }
                }
            }
            
            return balances;
        }

        /// <summary>
        ///  Get the balance for the user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserBalance GetUserBalance(int userId)
        {
            UserAccount user = users[userId];
            UserBalance balance = new UserBalance(user);

            decimal groupTotal = this.MoneyTotal;
            decimal groupShare = groupTotal / users.Count;
            
            decimal moneySpent = GetUserSpent(user.id);

            balance.balance = Math.Round(moneySpent - groupShare, 2);
            balance.balanceTotal = balance.balance;

            return balance;
        }
    }

    public class UserBalance
    {
        public UserAccount user;
        public decimal balanceTotal = 0;
        public decimal balance = 0;
        public Dictionary<int, Debt> debt = new Dictionary<int, Debt>();

        public UserBalance(UserAccount user)
        {
            this.user = user;
        }
    }

    public class Debt
    {
        public decimal amount = 0;
        public List<Expense> expenses = new List<Expense>();
    }
}
