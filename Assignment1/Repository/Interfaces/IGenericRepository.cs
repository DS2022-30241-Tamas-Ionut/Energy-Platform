namespace Assignment1.Repository.Interfaces
{
    public interface IGenericRepository
    {
        List<T> GetAll<T>() where T : class;
        T Get<T>(int id) where T : class;
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
    }
}
