
namespace Trifork_Coding_Assignment
{
    
    public class Group
    {
        public string? groupName = null;
        public Dictionary<int, UserAccount> users = new Dictionary<int, UserAccount>();
        private Dictionary<int, UserBalance> userbalances = new Dictionary<int, UserBalance>();

        private List<Expense> expenses = new List<Expense>();

        public double MoneyTotal
        {
            get {
                double total = 0;

                foreach (Expense expense in expenses)
                {
                    total += expense.isCompleted() ? 0 : expense.price;
                }

                return total;
            }
        }

        /// <summary>
        ///  Get all users
        /// </summary>
        public List<UserAccount> Users
        {
            get
            {
                return new List<UserAccount>(this.users.Values);
            }
        }

        /// <summary>
        ///  Get all unpaid expenses 
        /// </summary>
        public List<Expense> UnpaidExpenses
        {
            get
            {
                return expenses.FindAll(exp => !exp.isCompleted());
            }
        }

        /// <summary>
        ///  Get all expenses 
        /// </summary>
        public List<Expense> Expenses
        {
            get
            {
                return expenses;
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
        public void AddExpense(int userId, double price, string name)
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
        public double GetUserSpent(int userId)
        {
            double total = 0;

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
        ///  Send a payment to a user
        /// </summary>
        /// <param name="userId1"></param>
        /// <param name="userId2"></param>
        public void sendPayments(int userId1, int userId2)
        {
            List<Expense> expenses = GetAllUnPaidExpenses(userId1);
            UserBalance payments = GetListOfPayments().Find(balance => balance.user.id == userId1);

            if (payments != null)
            {
                Debt deb = payments.debt[userId2];

                if (userbalances[userId1].debt.ContainsKey(userId2))
                {
                    userbalances[userId1].debt[userId2].amount += deb.amount;
                }
                else
                {
                    Debt debt = new Debt();
                    debt.amount = deb.amount;
                    userbalances[userId1].debt[userId2] = debt;
                }
            }

            foreach (Expense expense in expenses)
            {
                expense.markAsPaid(userId1);
            }
        }

        /// <summary>
        ///  Mark all payments has been made
        /// </summary>
        public void complete()
        {
            foreach (Expense expense in UnpaidExpenses)
            {
                foreach(UserAccount userAccount in Users)
                {
                    expense.markAsPaid(userAccount.id);
                }
            }
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

            foreach (UserBalance balance in balances)
            {
                // this user need money from the others
                if(balance.balance > 0)
                {
                    foreach (UserBalance balance2 in balances)
                    {
                        if (balance.balance <= 0)
                        {
                            break;
                        }

                        // this user need money from the others
                        if (balance2.user.id != balance.user.id && balance2.balance < 0)
                        {
                            if (Math.Abs(balance2.balance) == balance.balance)
                            {
                                double amount = Math.Abs(balance2.balance);
                                Debt debt = new Debt();
                                debt.amount = amount;
                                balance2.debt.Add(balance.user.id, debt);

                                balance2.balance = 0;
                                balance.balance = 0;
                              // user2 balance is less user1 balance 
                            } else if (Math.Abs(balance2.balance) <= balance.balance)
                            {
                                double amount = Math.Abs(balance2.balance);
                                Debt debt = new Debt();
                                debt.amount = amount;
                                balance2.debt.Add(balance.user.id, debt);

                                balance2.balance = 0;
                                balance.balance = balance.balance - amount;
                            } else
                            {
                                double amount = Math.Abs(balance2.balance) - balance.balance;
                                Debt debt = new Debt();
                                debt.amount = amount;

                                balance2.debt.Add(balance.user.id, debt);
                                balance2.balance = -amount;
                                balance.balance = 0;
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

            double groupTotal = this.MoneyTotal;
            double groupShare = Math.Round(groupTotal / users.Count, 3);

            double moneySpent = GetUserSpent(user.id);

            balance.balance = Math.Round(moneySpent - groupShare, 2);
            balance.balanceTotal = balance.balance;

            return balance;
        }
    }

    public class UserBalance
    {
        public UserAccount user;
        public double balanceTotal = 0;
        public double balance = 0;
        public Dictionary<int, Debt> debt = new Dictionary<int, Debt>();

        public UserBalance(UserAccount user)
        {
            this.user = user;
        }

        public override string ToString()
        {
            return String.Format("User(id: {0}, name: {1}) balance: {2}$", user?.id, user?.userName, balance );
        }
    }

    public class Debt
    {
        public double amount = 0;
        public List<Expense> expenses = new List<Expense>();
    }
}
