using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankSystem.Test
{
    /// <summary>
    /// Integration Tests
    /// </summary>
    [TestClass]
    public class IntegrationTest
    {
        IRepository repository;
        IBank bank;

        [TestInitialize()]
        public void Initialize()
        {
            repository = new AccountRepository();
            bank = new Bank(repository);
            repository.CleanupAccounts();           
        }

        [TestCleanup()]
        public void Cleanup()
        { 
            repository.CleanupAccounts();
        }

        [TestMethod]
        public void CreateNewAccount_IsSuccessful()
        {
            // Arrange
            var loginName = "TestUser1";
            var password = "Password1";
            var initialDeposit = 1000;

            // Act
            var newAccount = bank.CreateAccount(loginName, password, initialDeposit);

            // Assert
            Assert.IsNotNull(newAccount, "Account is null.");
            Assert.AreEqual(loginName, newAccount.LoginName);
            Assert.AreEqual(password, newAccount.Password);
            Assert.AreEqual(initialDeposit, newAccount.Balance);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateAccount_WithNullOrEmptyLoginName_ThrowsArgumentNullException()
        {
            // Arrange
            var loginName = "";
            var password = "Password1";
            var initialDeposit = 1000;

            // Act
            var newAccount = bank.CreateAccount( loginName, password, initialDeposit);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateAccount_WithNullOrEmptyPassword_ThrowsArgumentNullException()
        {
            // Arrange
            var loginName = "TestUser1";
            var password = "";
            var initialDeposit = 1000;

            // Act
            var newAccount = bank.CreateAccount( loginName, password, initialDeposit);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Deposit_WithNullAccount_ThrowsArgumentNullException()
        {
            // Arrange
            var amount = 1234.56;
            
            // Act
            bank.Deposit(null, amount);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Withdraw_WithNullAccount_ThrowsArgumentNullException()
        {
            // Arrange
            var amount = 1234.56;

            // Act
            bank.Withdraw(null, amount);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Transfer_WithNullFromAccount_ThrowsArgumentNullException()
        {
            // Arrange
            var amount = 1234.56;
            var loginName = "";
            var password = "Password1";
            var initialDeposit = 1000;
            var toAccount = bank.CreateAccount( loginName, password, initialDeposit);

            // Act
            bank.Transfer(null, toAccount, amount);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Transfer_WithNullToAccount_ThrowsArgumentNullException()
        {
            // Arrange
            var amount = 1234.56;
            var loginName = "";
            var password = "Password1";
            var initialDeposit = 1000;
            var fromAccount = bank.CreateAccount( loginName, password, initialDeposit);

            // Act
            bank.Transfer(fromAccount, null, amount);

            // Assert
        }

        [TestMethod]
        public void LoginNameAlreadyExists()
        {
            // Arrange
            var loginName = "TestUser1";
            var password = "Password1";
            var initialDeposit = 1000;

            // Act
            var newAccount = bank.CreateAccount( loginName, password, initialDeposit);

            // Act
            var result = repository.IsLoginExists(loginName);

            // Assert
            Assert.IsTrue(result, $"Login Name {loginName} doesn't exists.");
        }

        [TestMethod]
        public void Deposit_AmountToAccount_IsSuccessful()
        {
            // Sender
            
            var customerLoginName = "Customer1";
            var customerPassword = "Password1";
            var customerInitialBalance = 10000;
            var customerAccount = bank.CreateAccount(customerLoginName, customerPassword, customerInitialBalance);

            var amountToDeposit = 1234.56;

            // Act
            bank.Deposit(customerAccount, amountToDeposit);

            // Assert
            customerAccount = repository.Get(customerAccount.AccountNumber.ToString());
            Assert.IsNotNull(customerAccount, "Account is null");
            Assert.AreEqual(customerInitialBalance + amountToDeposit, customerAccount.Balance);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ConcurrencyTest_ThrowsException()
        {
            // Sender

            var customerLoginName = "Customer1";
            var customerPassword = "Password1";
            var customerInitialBalance = 5000;
            var customerAccount = bank.CreateAccount(customerLoginName, customerPassword, customerInitialBalance);

            var amountToDeposit = 1000;

            // Receiver
            var ReceiverLoginName = "Customer1101";
            var ReceiverPassword = "Password1";
            var ReceiverInitialBalance = 0;
            var ReceiverAccount = bank.CreateAccount(ReceiverLoginName, ReceiverPassword, ReceiverInitialBalance);


            // Act
            Task[] taskArray = new Task[4];
            // Deposit 1000
            taskArray[0] = Task.Factory.StartNew(() => {
                bank.Deposit(customerAccount, amountToDeposit);
                    }
                );
            
            // Transfer 1000 to Receiver 
            taskArray[1] = Task.Factory.StartNew(() => {
                bank.Transfer(customerAccount, ReceiverAccount, amountToDeposit);
                    }
                );

            // Withdraw
            taskArray[2] = Task.Factory.StartNew(() => {
                bank.Withdraw(customerAccount, amountToDeposit);
            }
                );
            
            // Withdraw
            taskArray[3] = Task.Factory.StartNew(() => {
                bank.Withdraw(ReceiverAccount, 100);
            }
                );

            Task.WaitAll(taskArray);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Deposit_WhenAmountToDepositIsLessThatOrEqualToZero_ThrowsException()
        {
            // Sender
            
            var customerLoginName = "customer1";
            var customerPassword = "Password1";
            var customerInitialBalance = 10000;
            var customerAccount = bank.CreateAccount( customerLoginName, customerPassword, customerInitialBalance);

            var amountToDeposit = 0;

            // Act
            bank.Deposit(customerAccount, amountToDeposit);

            // Assert
        }

        [TestMethod]
        public void Withdraw_WhenAmountIsLessThanOrEqualToAccountBalance_IsSuccessful()
        {
            // Sender
            
            var customerLoginName = "Customer1";
            var customerPassword = "Password1";
            var customerInitialBalance = 10000;
            var customerAccount = bank.CreateAccount( customerLoginName, customerPassword, customerInitialBalance);

            var amountToWithdraw = 1234.56;

            // Act
            bank.Withdraw(customerAccount, amountToWithdraw);

            // Assert
            customerAccount = repository.Get(customerAccount.AccountNumber.ToString());
            Assert.IsNotNull(customerAccount, "Account is null");
            Assert.AreEqual(customerInitialBalance - amountToWithdraw, customerAccount.Balance);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Withdraw_WhenAmountIsLessThanOrEqualToZero_ThrowsException()
        {
            // Sender
            
            var customerLoginName = "customer1";
            var customerPassword = "Password1";
            var customerInitialBalance = 10000;
            var customerAccount = bank.CreateAccount( customerLoginName, customerPassword, customerInitialBalance);

            var amountToWithdraw = 0;

            // Act
            bank.Withdraw(customerAccount, amountToWithdraw);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Insufficient Funds.")]
        public void Withdraw_WhenAmountIsGreaterThanAccountBalance_ThrowsException()
        {
            // Sender
            
            var customerLoginName = "Customer1";
            var customerPassword = "Password1";
            var customerInitialBalance = 10000;
            var customerAccount = bank.CreateAccount(customerLoginName, customerPassword, customerInitialBalance);

            var amountToDeposit = 11234.56;

            // Act
            bank.Withdraw(customerAccount, amountToDeposit);

            // Assert       
        }


        [TestMethod]
        public void Transfer_AmountFromSenderToReceiverAccount_IsSuccessful()
        {
            // Sender

            var senderLoginName = "Sender1";
            var senderPassword = "Password1";
            var senderInitialBalance = 10000;           
            var senderAccount = bank.CreateAccount(senderLoginName, senderPassword, senderInitialBalance);

            // Receiver

            var receiverLoginName = "Receiver1";
            var receiverPassword = "Password1";
            var receiverInitialBalance = 0;
            var receiverAccount = bank.CreateAccount(receiverLoginName, receiverPassword, receiverInitialBalance);

            var amountToTransfer = 1234.56;

            // Act
            bank.Transfer(senderAccount, receiverAccount, amountToTransfer);

            // Assert
            senderAccount = repository.Get(senderAccount.AccountNumber);
            Assert.AreEqual(senderInitialBalance - amountToTransfer, senderAccount.Balance);

            receiverAccount = repository.Get(receiverAccount.AccountNumber);
            Assert.AreEqual(receiverInitialBalance + amountToTransfer, receiverAccount.Balance);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Insufficient Funds.")]
        public void Transfer_WhenAmountIsGreaterThanAccountBalance_ThrowsException()
        {
            // Sender

            var senderLoginName = "Sender1";
            var senderPassword = "Password1";
            var senderInitialBalance = 10000;
            var senderAccount = bank.CreateAccount(senderLoginName, senderPassword, senderInitialBalance);

            // Receiver
            var receiverLoginName = "Receiver1";
            var receiverPassword = "Password1";
            var receiverInitialBalance = 0;
            var receiverAccount = bank.CreateAccount(receiverLoginName, receiverPassword, receiverInitialBalance);

            var amountToTransfer = 11234.56;

            // Act
            bank.Transfer(senderAccount, receiverAccount, amountToTransfer);

            // Assert
        }
    }
}
