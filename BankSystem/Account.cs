using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankSystem
{
    public class Account
    {
        public int Id { get; set; }
        public string LoginName { get; set; }
        public string AccountNumber { get; set; }
        public string Password { get; set; }
        public double Balance { get; set; }
        public DateTime CreatedDate { get; set; }

        public Account()
        {

        }

        public Account(string loginName, string password, double initialBalance = 0)
        {
            if (string.IsNullOrEmpty(loginName)) throw new ArgumentNullException("Login Name is null or empty");
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException("Password is null or empty");

            AccountNumber = $"Account-{Guid.NewGuid()}"; 
            LoginName = loginName;
            Password = password;
            Balance = initialBalance;
            CreatedDate = DateTime.Now;
        }

        public Account Deposit(double amount)
        {
            this.Balance += amount;
            return this;
        }

        public Account Withdraw(double amount)
        {
            if (amount > this.Balance) throw new Exception("Insufficient Funds.");

            if (amount <= this.Balance)
                this.Balance -= amount;

            return this;
        }
    }
}
