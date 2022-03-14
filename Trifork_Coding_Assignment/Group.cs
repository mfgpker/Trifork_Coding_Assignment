
namespace Trifork_Coding_Assignment
{
    
    public class Group
    {
        public string? groupName = null;
        public Dictionary<int, UserAccount> users = new Dictionary<int, UserAccount>();

        public List<Expense> expenses = new List<Expense>();

        public Group(String groupName, List<UserAccount> usersList)
        {
            this.groupName = groupName;

            foreach(var user in usersList)
            {
                this.users.Add(user.id, user);
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
        ///     The user pay all unpaid expenses the user has
        /// </summary>
        /// <param name="userId"></param>
        public void userPayAllDebt(int userId)
        {
            if (users[userId] == null)
            {
                return;
            }

            UserAccount userAccount = users[userId];
            UserBalance balance = GetUserBalance(userId);

            // user transfere money
            foreach (KeyValuePair<int, Debt> depKvp in balance.debt)
            {
                userAccount.TransfereMoneyTo(users[depKvp.Key], depKvp.Value.amount);
            }

            // mark all expenses that the user has paid the part the user owns
            foreach(Expense expense in GetAllUnPaidExpenses(userId))
            {
                expense.markAsPaid(userId);
            }
        }

        /// <summary>
        ///  A user pay its share of the expense
        /// </summary>
        /// <param name="expense"></param>
        /// <param name="userId"></param>
        public void userPayShare(Expense expense, int userId)
        {
            if (expense != null && users[userId] != null)
            {
                expense.userPay(users[userId]);
            }
        }

        /// <summary>
        ///  Check balance for each user in the group
        /// </summary>
        /// <param name="getRawBalance"></param>
        /// 
        /// <returns>Get a balance for each users </returns>
        public List<UserBalance> checkBalance(bool getRawBalance)
        {
            Dictionary<int, UserBalance> balances = new Dictionary<int, UserBalance>();

            foreach(KeyValuePair<int, UserAccount> userKvp in users)
            {
                UserAccount user = userKvp.Value;
                UserBalance balance = GetUserBalance(user.id);

                balances.Add(user.id, balance);
            }

            if (getRawBalance != true)
            {
                // check if users own money to each other
                foreach (KeyValuePair<int, UserBalance> balanceKvp in balances)
                {
                    UserBalance user1Balance = balanceKvp.Value;
                    UserAccount user1 = users[balanceKvp.Key];

                    // check if the 2 users owe each other
                    foreach (KeyValuePair<int, Debt> userKvp in user1Balance.debt)
                    {
                        UserAccount user2 = users[userKvp.Key];
                        UserBalance user2Balance = balances[userKvp.Key];

                        if (user2Balance.debt.ContainsKey(user1.id))
                        {
                            // 
                            if (user1Balance.debt[user2.id] == user2Balance.debt[user1.id])
                            {
                                user1Balance.total -= user1Balance.debt[user2.id].amount;
                                user2Balance.total -= user2Balance.debt[user1.id].amount;

                                user1Balance.debt[user2.id].amount = 0;
                                user2Balance.debt[user1.id].amount = 0;

                                user1Balance.debt.Remove(user2.id);
                                user2Balance.debt.Remove(user1.id);
                            }
                            // user1 owes user2 less than user2 owes user1
                            else if (user1Balance.debt[user2.id].amount < user2Balance.debt[user1.id].amount)
                            {
                                user1Balance.total -= user1Balance.debt[user2.id].amount;
                                user2Balance.total -= user1Balance.debt[user2.id].amount;

                                user2Balance.debt[user1.id].amount = user2Balance.debt[user1.id].amount - user1Balance.debt[user2.id].amount;
                                user1Balance.debt[user2.id].amount = 0;

                                user1Balance.debt.Remove(user2.id);
                            }
                            // user owes userDebtTo more than userDebtTo owes user
                            else if (user1Balance.debt[user2.id].amount > user2Balance.debt[user1.id].amount)
                            {

                                user1Balance.total -= user2Balance.debt[user1.id].amount;
                                user2Balance.total -= user2Balance.debt[user1.id].amount;

                                user1Balance.debt[user2.id].amount = user1Balance.debt[user2.id].amount - user2Balance.debt[user1.id].amount;
                                user2Balance.debt[user1.id].amount = 0;

                                user2Balance.debt.Remove(user1.id);
                            }
                        }
                    }
                }
            }

            return new List<UserBalance>(balances.Values);
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

            foreach (Expense expense in expenses)
            {
                if (!expense.isCompleted() && expense.paidByUserId != user.id && !expense.hasUserPaid(user))
                {
                    double amount = expense.getAmount(user);
                    balance.total += amount;

                    if (balance.debt.ContainsKey(expense.paidByUserId))
                    {
                        balance.debt[expense.paidByUserId].amount += amount;
                        balance.debt[expense.paidByUserId].expenses.Add(expense);
                    }
                    else
                    {
                        Debt debt = new Debt
                        {
                            amount = amount
                        };
                        debt.expenses.Add(expense);
                        balance.debt.Add(expense.paidByUserId, debt);
                    }
                }
            }

            return balance;
        }
    }

    public class UserBalance
    {
        public UserAccount user;
        public double total = 0;
        public Dictionary<int, Debt> debt = new Dictionary<int, Debt>();

        public UserBalance(UserAccount user)
        {
            this.user = user;
        }
    }

    public class Debt
    {
        public double amount = 0;
        public List<Expense> expenses = new List<Expense>();
    }
}
