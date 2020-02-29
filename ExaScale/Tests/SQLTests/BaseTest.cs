using System.Transactions;

namespace UnitTests
{
    public class BaseTest
    {
        protected TransactionScope transaction;

        public System.Transactions.TransactionScope GetTransaction()
        {
            this.transaction = new System.Transactions.TransactionScope(TransactionScopeOption.RequiresNew);
            return transaction;
        }
    }
}