using System;

namespace BankSystem
{
    public interface IBank
    {
        Account CreateAccount(string loginName, string password, double initialDeposit = 0);
        void Deposit(Account toAccount, double amount);
        void Withdraw(Account fromAccount, double amount);
        void Transfer(Account fromAccount, Account toAccount, double amount);
        Account GetAccount(string accountNumber);
    }

    public class Bank : IBank
    {
        IRepository accountRepository;

        public Bank(IRepository repository)
        {
            this.accountRepository = repository ?? new AccountRepository();
        }

        public Account GetAccount(string accountNumber)
        {
            return accountRepository.Get(accountNumber);
        }

        public Account CreateAccount(string loginName, string password, double initialDeposit = 0)
        {            
            var newAccount = new Account(loginName, password, initialDeposit);

            if (accountRepository.IsLoginExists(loginName))
                throw new Exception($"Login Name {loginName} already exists.");

            return accountRepository.Create(newAccount);
        }

        public void Deposit(Account toAccount, double amount)
        {
            if (toAccount == null) throw new ArgumentNullException($"Account is null.");
            if (amount <= 0) throw new ArgumentException("Amount is zero.");

            var customerAccount = accountRepository.Get(toAccount.AccountNumber);
            if (customerAccount == null) throw new Exception($"Account No. {toAccount.AccountNumber} doesn't exists.");
            customerAccount.Deposit(amount);

            accountRepository.Update(customerAccount);
        }

        public void Transfer(Account fromAccount, Account toAccount, double amount)
        {
            if (fromAccount == null) throw new ArgumentNullException($"From Account is null.");
            if (toAccount == null) throw new ArgumentNullException($"To Account is null.");

            accountRepository.Transfer(fromAccount.Withdraw(amount), toAccount.Deposit(amount));
        }

        public void Withdraw(Account fromAccount, double amount)
        {
            if (fromAccount == null) throw new ArgumentNullException($"From Account is null.");
            if (amount <= 0) throw new ArgumentException("Amount is zero.");

            var customerAccount = accountRepository.Get(fromAccount.AccountNumber);
            if (customerAccount == null) throw new Exception($"Account No. {fromAccount.AccountNumber} doesn't exists.");
            customerAccount.Withdraw(amount);
    
            accountRepository.Update(customerAccount);
        }
    }
}
