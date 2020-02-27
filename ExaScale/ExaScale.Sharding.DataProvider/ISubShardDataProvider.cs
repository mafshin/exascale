namespace ExaScale.Sharding.DataProvider
{
    public interface ISubShardDataProvider
    {
        long GetShardItemsCount();
        void Initialize(string connection);
    }
}
