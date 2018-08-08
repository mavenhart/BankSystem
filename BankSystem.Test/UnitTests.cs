using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankSystem.Test
{
    [TestClass]
    public class UnitTests
    {     
        public IRepository MockAccountRepository;

        [TestMethod]
        public void CanCreateNewAccount()
        {
            var expectedAccount = new Account
            {
                Id = 1,
                LoginName = "Customer101",
                AccountNumber = $"ACC-{Guid.NewGuid()}",
                Password = "Passowrd12",
                Balance = 1000,
                CreatedDate = DateTime.Now
            };

            var mockAccountRepo = new Mock<IRepository>();
            mockAccountRepo.Setup(account => account.Create(It.IsAny<Account>())).Returns(expectedAccount);

            this.MockAccountRepository = mockAccountRepo.Object;

            var bank = new Bank(this.MockAccountRepository);
            var newAccount = bank.CreateAccount(expectedAccount.LoginName, expectedAccount.Password, 1000);

            Assert.IsNotNull(newAccount); // Test if null
            Assert.IsInstanceOfType(newAccount, typeof(Account)); // Test type
            Assert.AreEqual(newAccount.AccountNumber, expectedAccount.AccountNumber);
        }

        [TestMethod]
        public void CanGetAccountByAccountNo()
        {
            var expectedAccount = new Account
            {
                Id = 1,
                LoginName = "Customer101",
                AccountNumber = $"ACC-{Guid.NewGuid()}",
                Balance = 1000,
                CreatedDate = DateTime.Now
            };

            var mockAccountRepo = new Mock<IRepository>();
            mockAccountRepo.Setup(account => account.Get(It.IsAny<string>())).Returns(expectedAccount);

            this.MockAccountRepository = mockAccountRepo.Object;

            var customerAccount = MockAccountRepository.Get(expectedAccount.AccountNumber);

            Assert.IsNotNull(customerAccount); // Test if null
            Assert.IsInstanceOfType(customerAccount, typeof(Account)); // Test type
            Assert.AreEqual(customerAccount.AccountNumber, expectedAccount.AccountNumber);
        }

        [TestMethod]
        public void CanDepositToAccount()
        {
            var expectedAccount = new Account
            {
                Id = 1,
                LoginName = "Customer101",
                AccountNumber = $"ACC-{Guid.NewGuid()}",
                Balance = 1000,
                CreatedDate = DateTime.Now
            };

            var mockAccountRepo = new Mock<IRepository>();
            mockAccountRepo.Setup(account => account.Update(It.IsAny<Account>()));
            mockAccountRepo.Setup(account => account.Get(It.IsAny<string>())).Returns(expectedAccount);    
            this.MockAccountRepository = mockAccountRepo.Object;

            var bank = new Bank(this.MockAccountRepository);
            bank.Deposit(expectedAccount, 100);

            Assert.AreEqual(1100, expectedAccount.Balance);

            mockAccountRepo.Verify(account => account.Update(It.IsAny<Account>()), Times.Once());            
        }

        [TestMethod]
        public void CanWithdrawFromAccount()
        {
            var expectedAccount = new Account
            {
                Id = 1,
                LoginName = "Customer101",
                AccountNumber = $"ACC-{Guid.NewGuid()}",
                Balance = 1000,
                CreatedDate = DateTime.Now
            };

            var mockAccountRepo = new Mock<IRepository>();
            mockAccountRepo.Setup(account => account.Update(It.IsAny<Account>()));
            mockAccountRepo.Setup(account => account.Get(It.IsAny<string>())).Returns(expectedAccount);
            this.MockAccountRepository = mockAccountRepo.Object;

            var bank = new Bank(this.MockAccountRepository);
            bank.Withdraw(expectedAccount, 100);

            Assert.AreEqual(900, expectedAccount.Balance);

            mockAccountRepo.Verify(account => account.Update(It.IsAny<Account>()), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Insufficient Funds.")]
        public void CannotWithdrawFromAccountWhenBalanceIsLessThanWithdrawAmount_ThrowsException()
        {
            var expectedAccount = new Account
            {
                Id = 1,
                LoginName = "Customer101",
                AccountNumber = $"ACC-{Guid.NewGuid()}",
                Balance = 1000,
                CreatedDate = DateTime.Now
            };

            var mockAccountRepo = new Mock<IRepository>();
            mockAccountRepo.Setup(account => account.Update(It.IsAny<Account>()));
            mockAccountRepo.Setup(account => account.Get(It.IsAny<string>())).Returns(expectedAccount);
            this.MockAccountRepository = mockAccountRepo.Object;

            var bank = new Bank(this.MockAccountRepository);
            bank.Withdraw(expectedAccount, 1200);

            mockAccountRepo.Verify(account => account.Update(It.IsAny<Account>()), Times.Once());
        }

        [TestMethod]
        public void CanTransferAmountToAccount()
        {
            var sender = new Account
            {
                Id = 1,
                LoginName = "Customer101",
                AccountNumber = $"ACC-{Guid.NewGuid()}",
                Balance = 1000,
                CreatedDate = DateTime.Now
            };

            var receiver = new Account
            {
                Id = 2,
                LoginName = "Customer201",
                AccountNumber = $"ACC-{Guid.NewGuid()}",
                Balance = 3000,
                CreatedDate = DateTime.Now
            };


            var mockAccountRepo = new Mock<IRepository>();
            mockAccountRepo.Setup(account => account.Transfer(It.IsAny<Account>(), It.IsAny<Account>()));
            
            this.MockAccountRepository = mockAccountRepo.Object;

            var bank = new Bank(this.MockAccountRepository);
            bank.Transfer(sender, receiver, 888);

            Assert.AreEqual(112, sender.Balance);
            Assert.AreEqual(3888, receiver.Balance);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Insufficient Funds.")]
        public void CannotTransferAmountToAccountWhenAmountIsGreaterThanBalance_ThrowsException()
        {
            var sender = new Account
            {
                Id = 1,
                LoginName = "Customer101",
                AccountNumber = $"ACC-{Guid.NewGuid()}",
                Balance = 1000,
                CreatedDate = DateTime.Now
            };

            var receiver = new Account
            {
                Id = 2,
                LoginName = "Customer201",
                AccountNumber = $"ACC-{Guid.NewGuid()}",
                Balance = 3000,
                CreatedDate = DateTime.Now
            };


            var mockAccountRepo = new Mock<IRepository>();
            mockAccountRepo.Setup(account => account.Transfer(It.IsAny<Account>(), It.IsAny<Account>()));

            this.MockAccountRepository = mockAccountRepo.Object;

            var bank = new Bank(this.MockAccountRepository);
            bank.Transfer(sender, receiver, 1500);

        }
    }
}
