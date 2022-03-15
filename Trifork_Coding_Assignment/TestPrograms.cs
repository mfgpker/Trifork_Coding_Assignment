namespace Trifork_Coding_Assignment
{
    internal class TestPrograms
    {
        public virtual void run () {
            Console.WriteLine("\n------------------- RUNNING TEST 1 -------------------\n");
            new TestProgram1().run();

            Console.WriteLine("\n------------------- RUNNING TEST 2 -------------------\n");
            new TestProgram2().run();

            Console.WriteLine("\n------------------- RUNNING TEST 3 -------------------\n");
            new TestProgram3().run();

            Console.WriteLine("\n------------------- RUNNING TEST 4 -------------------\n");
            new TestProgram4().run();
        }

        public UserAccount CreateUser(int id)
        {
            double money = 1000;
            String userName = "TEST-USER-" + id.ToString();

            return new UserAccount(id, userName, money);
        }

        public void PrintListOfPayments(Group group)
        {
            List<UserBalance> balances = group.GetListOfPayments();

            Console.WriteLine("---------- List of payments ----------");
            foreach (UserBalance balance in balances)
            {
                Console.WriteLine("user (id: " + balance.user.id + ", " + balance.user.userName + ")");
                Console.WriteLine("balance: " + balance.balance + "$");

                foreach (KeyValuePair<int, Debt> debt in balance.debt)
                {
                    Console.WriteLine("  " + debt.Value.amount + "$ own to user (id: " + debt.Key + ")");
                }

                Console.WriteLine("---------------------------\n");
            }
        }
    }

    internal class TestProgram1 : TestPrograms
    {
        public override void run()
        {
            UserAccount john = new UserAccount(1, "John", 10000);
            UserAccount peter = new UserAccount(2, "Peter", 10000);
            UserAccount mary = new UserAccount(3, "Mary", 10000);

            List<UserAccount> users = new List<UserAccount>();
            users.Add(john); users.Add(peter); users.Add(mary);

            Group group = new Group("holiday - trip", users);

            group.AddExpense(john.id, 500, "Hotel");
            group.AddExpense(mary.id, 150, "Restaurant");
            group.AddExpense(peter.id, 100, "sightseeing");

            PrintListOfPayments(group);
        }
    }

    internal class TestProgram2 : TestPrograms
    {
        public override void run()
        {
            UserAccount user1 = CreateUser(1);
            UserAccount user2 = CreateUser(2);

            List<UserAccount> users = new List<UserAccount>();
            users.Add(user1); users.Add(user2);
            Group group = new Group("test-group", users);

            Console.WriteLine("---------- Users ----------");
            Console.WriteLine(user1.ToString());
            Console.WriteLine(user2.ToString());

            group.AddExpense(user1.id, 40, "Cafe 1");
            group.AddExpense(user2.id, 70, "Cafe 2");
            group.AddExpense(user2.id, 160, "fuel");

            PrintListOfPayments(group);

            Console.WriteLine("---------- Get User Balance ----------");
            foreach (UserAccount user in group.Users)
            {
                Console.WriteLine(group.GetUserBalance(user.id));
            }

            Console.WriteLine("---------- Get Expenses ----------");

            foreach (Expense expense in group.Expenses)
            {
                Console.WriteLine(expense.ToString());
            }
        }
    }


    internal class TestProgram3 : TestPrograms
    {
        public override void run()
        {
            UserAccount user1 = CreateUser(1);
            UserAccount user2 = CreateUser(2);

            List<UserAccount> users = new List<UserAccount>();
            users.Add(user1); users.Add(user2);
            Group group = new Group("test-group", users);

            Console.WriteLine("---------- Users ----------");
            Console.WriteLine(user1.ToString());
            Console.WriteLine(user2.ToString());

            group.AddExpense(user1.id, 70, "Cafe 1");
            group.AddExpense(user1.id, 160, "fuel 1");

            group.AddExpense(user2.id, 70, "Cafe 2");
            group.AddExpense(user2.id, 160, "fuel2");


            Console.WriteLine("---------- Get User Balance ----------");
            foreach (UserAccount user in group.Users)
            {
                Console.WriteLine(group.GetUserBalance(user.id));
            }

            PrintListOfPayments(group);

            Console.WriteLine("---------- Get Expenses ----------");
            foreach (Expense expense in group.Expenses)
            {
                Console.WriteLine(expense.ToString());
            }

        }
    }

    internal class TestProgram4 : TestPrograms
    {
        public override void run()
        {
            UserAccount user1 = CreateUser(1);
            UserAccount user2 = CreateUser(2);
            UserAccount user3 = CreateUser(3);
            UserAccount user4 = CreateUser(4);
            UserAccount user5 = CreateUser(5);
            UserAccount user6 = CreateUser(6);
            UserAccount user7 = CreateUser(7);
            UserAccount user8 = CreateUser(8);
            UserAccount user9 = CreateUser(9);
            UserAccount user10 = CreateUser(10);
            List<UserAccount> users = new List<UserAccount>();
            users.Add(user1); users.Add(user2); users.Add(user3); users.Add(user4); users.Add(user5);
            users.Add(user6); users.Add(user7); users.Add(user8); users.Add(user9); users.Add(user10);

            Group group = new Group("Big-test-group", users);

            group.AddExpense(user1.id, 100, "Cafe 1");
            group.AddExpense(user2.id, 70, "Cafe 2");
            group.AddExpense(user2.id, 160, "fuel");
            group.AddExpense(user3.id, 160, "Hotal");
            group.AddExpense(user4.id, 250, "flight ticket");
            group.AddExpense(user5.id, 100, "Renting cars");
            group.AddExpense(user5.id, 150, "Bar trip");
            group.AddExpense(user7.id, 30, "hangour pizza");
            group.AddExpense(user10.id, 200, "taxi");

            Console.WriteLine("---------- Get User Balance ----------");
            foreach (UserAccount user in group.Users)
            {
                Console.WriteLine(group.GetUserBalance(user.id));
            }

            PrintListOfPayments(group);

            foreach (Expense expense in group.Expenses)
            {
                Console.WriteLine(expense.ToString());
            }
        }
    }
}
