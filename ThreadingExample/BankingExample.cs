using System;
using System.Threading;

namespace ThreadingExample
{
    class BankingExample
    {
        public static void BankMain()
        {
            Console.WriteLine("Main Started");
            Account accountA = new Account(101, 5000);
            Account accountB = new Account(102, 3000);

            AccountManager accountManagerA = new AccountManager(accountA, accountB, 1000);
            Thread T1 = new Thread(accountManagerA.Transfer)
            {
                Name = "T1"
            };

            AccountManager accountManagerB = new AccountManager(accountB, accountA, 2000);
            Thread T2 = new Thread(accountManagerB.Transfer)
            {
                Name = "T2"
            };

            T1.Start();
            T2.Start();

            T1.Join();
            T2.Join();
            Console.WriteLine("Main Completed");
        }
    }

    public class Account
    {
        private double _balance;
        private readonly int _id;
        public Account(int id, double balance)
        {
            _id = id;
            _balance = balance;
        }
        public int ID
        {
            get
            {
                return _id;
            }
        }

        public void Withdraw(double amount)
        {
            _balance -= amount;
        }

        public void Deposit(double amount)
        {
            _balance += amount;
        }
    }
    public class AccountManager
    {
        Account _from;
        Account _to;
        double _amount;

        public AccountManager(Account from, Account to, double amount)
        {
            _from = from;
            _to = to;
            _amount = amount;
        }

        public void TransferLock()
        {
            Console.WriteLine(Thread.CurrentThread.Name + " trying to acquire lock on " + _from.ID.ToString());
            lock (_from)
            {
                Console.WriteLine(Thread.CurrentThread.Name + " acquired lock on " + _from.ID.ToString());
                Console.WriteLine(Thread.CurrentThread.Name + " suspended for 1 second");
                Thread.Sleep(1000);
                Console.WriteLine(Thread.CurrentThread.Name + " back in action and trying to acquire lock on " + _to.ID.ToString());
                lock (_to)
                {
                    _from.Withdraw(_amount);
                    _to.Deposit(_amount);
                }
            }
        }

        public void Transfer()
        {
            object _lock1, _lock2;

            if (_from.ID < _to.ID)
            {
                _lock1 = _from;
                _lock2 = _to;
            }
            else
            {
                _lock1 = _to;
                _lock2 = _from;
            }

            Console.WriteLine(Thread.CurrentThread.Name + " trying to acquire lock on " + ((Account)_lock1).ID.ToString());

            lock (_lock1)
            {
                Console.WriteLine(Thread.CurrentThread.Name + " acquired lock on " + ((Account)_lock1).ID.ToString());
                Console.WriteLine(Thread.CurrentThread.Name + " suspended for 1 second");
                Thread.Sleep(1000);
                Console.WriteLine(Thread.CurrentThread.Name + " back in action and trying to acquire lock on " + ((Account)_lock2).ID.ToString());

                lock (_lock2)
                {
                    Console.WriteLine(Thread.CurrentThread.Name + " acquired lock on " + ((Account)_lock2).ID.ToString());
                    _from.Withdraw(_amount);
                    _to.Deposit(_amount);
                    Console.WriteLine(Thread.CurrentThread.Name + " Transfered " + _amount.ToString() + " from " + _from.ID.ToString() + " to " + _to.ID.ToString());
                }
            }
        }
    }
}
