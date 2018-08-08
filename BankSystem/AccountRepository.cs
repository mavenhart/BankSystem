using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;

namespace BankSystem
{
    public interface IRepository
    {
        Account Create(Account account);
        Account Get(string accountNo);
        void Update(Account account);
        void Delete(string accountNo);
        void Transfer(Account fromAccount, Account toAccount);
        bool IsLoginExists(string loginName);
        void CleanupAccounts();
    }

    public class AccountRepository : IRepository
    {
        private string ConnectionString { get; set; }

        public AccountRepository()
        {
            // TODO: Make this configurable
            this.ConnectionString = "Data Source=.\\SQLExpress;" +
         "Initial Catalog=BankSystem;" +
         "Integrated Security=SSPI;";
            
        }

        public AccountRepository(string connectionString)
        {
            if (String.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("ConnectionString is null or empty");

            ConnectionString = connectionString;
        }

        // CRUD        
        
        public Account Create(Account account)
        {
            try
            {
                // Create the TransactionScope to execute the commands, guaranteeing
                // that both commands can commit or roll back as a single unit of work.
                using (var scope = CreateTransactionScope())
                {
                    using (var connection = new SqlConnection(ConnectionString))
                    {
                        // Opening the connection automatically enlists it in the 
                        // TransactionScope as a lightweight transaction.
                        connection.Open();

                        var command1 = new SqlCommand($"INSERT INTO Account VALUES('{account.LoginName}', '{account.AccountNumber}', '{account.Password}', '{account.Balance}', '{account.CreatedDate}')", connection);
                        command1.ExecuteNonQuery();
                    }

                    // The Complete method commits the transaction. If an exception has been thrown,
                    // Complete is not called and the transaction is rolled back.
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                throw ex;
            }
            catch (ApplicationException ex)
            {
                throw ex;
            }

            return account;
        }        

        public Account Get(string accountNo)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    var command1 = new SqlCommand($"SELECT Id, LoginName, AccountNumber, Balance, CreatedDate FROM Account WHERE AccountNumber = '{accountNo}'", connection);
                    var reader = command1.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return new Account()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                LoginName = Convert.ToString(reader["LoginName"]),
                                AccountNumber = Convert.ToString(reader["AccountNumber"]),
                                Balance = Convert.ToDouble(reader["Balance"])                                
                            };
                        }
                    }

                    return null;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (ApplicationException ex)
            {
                throw ex;
            }
        }

        public void Update(Account account)
        {
            try
            {
                // Create the TransactionScope to execute the commands, guaranteeing
                // that both commands can commit or roll back as a single unit of work.
                using (var scope = CreateTransactionScope())
                {
                    using (var connection = new SqlConnection(ConnectionString))
                    {
                        // Opening the connection automatically enlists it in the 
                        // TransactionScope as a lightweight transaction.
                        connection.Open();

                        var command1 = new SqlCommand($"UPDATE Account SET Balance = {account.Balance} WHERE AccountNumber = '{account.AccountNumber}'", connection);
                        command1.ExecuteNonQuery();
                    }

                    // The Complete method commits the transaction. If an exception has been thrown,
                    // Complete is not called and the transaction is rolled back.
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                throw ex;
            }
            catch (ApplicationException ex)
            {
                throw ex;
            }
        }

        public bool IsLoginExists(string loginName)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    var command1 = new SqlCommand($"SELECT 1 FROM Account WHERE LoginName = '{loginName}'", connection);
                    var reader = command1.ExecuteReader();

                    if (reader.HasRows)
                        return true;

                    return false;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (ApplicationException ex)
            {
                throw ex;
            }
        }
        
        public void Delete(string accountNo)
        {
            try
            {
                // Create the TransactionScope to execute the commands, guaranteeing
                // that both commands can commit or roll back as a single unit of work.
                using (var scope = CreateTransactionScope())
                {
                    using (var connection = new SqlConnection(ConnectionString))
                    {
                        // Opening the connection automatically enlists it in the 
                        // TransactionScope as a lightweight transaction.
                        connection.Open();

                        var command1 = new SqlCommand($"DELETE FROM Account WHERE AccountNumber = '{accountNo}'" , connection);
                        command1.ExecuteNonQuery();
                    }

                    // The Complete method commits the transaction. If an exception has been thrown,
                    // Complete is not called and the transaction is rolled back.
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                throw ex;
            }
            catch (ApplicationException ex)
            {
                throw ex;
            }
        }

        public void Transfer(Account fromAccount, Account toAccount)
        {
            try
            {
                // Create the TransactionScope to execute the commands, guaranteeing
                // that both commands can commit or roll back as a single unit of work.
                using (var scope = CreateTransactionScope())
                {
                    using (var connection = new SqlConnection(ConnectionString))
                    {
                        // Opening the connection automatically enlists it in the 
                        // TransactionScope as a lightweight transaction.
                        connection.Open();

                        var command1 = new SqlCommand($"UPDATE Account SET Balance = {fromAccount.Balance} WHERE AccountNumber = '{fromAccount.AccountNumber}'", connection);
                        command1.ExecuteNonQuery();

                        var command2 = new SqlCommand($"UPDATE Account SET Balance = {toAccount.Balance} WHERE AccountNumber = '{toAccount.AccountNumber}'", connection);
                        command2.ExecuteNonQuery();
                    }

                    // The Complete method commits the transaction. If an exception has been thrown,
                    // Complete is not called and the transaction is rolled back.
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                throw ex;
            }
            catch (ApplicationException ex)
            {
                throw ex;
            }
        }

        // For Unit Tests only
        public void CleanupAccounts()
        {
            try
            {
                // Create the TransactionScope to execute the commands, guaranteeing
                // that both commands can commit or roll back as a single unit of work.
                using (var scope = CreateTransactionScope())
                {
                    using (var connection = new SqlConnection(ConnectionString))
                    {
                        // Opening the connection automatically enlists it in the 
                        // TransactionScope as a lightweight transaction.
                        connection.Open();

                        var command1 = new SqlCommand("TRUNCATE TABLE Account", connection);
                        command1.ExecuteNonQuery();
                    }

                    // The Complete method commits the transaction. If an exception has been thrown,
                    // Complete is not called and the transaction is rolled back.
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                throw ex;
            }
            catch (ApplicationException ex)
            {
                throw ex;
            }
        }

        private static TransactionScope CreateTransactionScope()
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = new TimeSpan(0, 10, 0) //assume 10 min is the timeout time
            };
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }
    }
}
