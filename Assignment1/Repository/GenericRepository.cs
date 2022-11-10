using Assignment1.Repository.Interfaces;

namespace Assignment1.Repository
{
    public class GenericRepository : IGenericRepository
    {
        private readonly EnergyUtilityDbContext _energyUltilityRepository;

        public GenericRepository(EnergyUtilityDbContext energyUltilityRepository)
        {
            _energyUltilityRepository = energyUltilityRepository;
        }

        public void Delete<T>(T entity) where T : class
        {
            _energyUltilityRepository.Set<T>().Remove(entity);
            _energyUltilityRepository.SaveChanges();
        }

        public T Get<T>(int id) where T : class
        {
            return _energyUltilityRepository.Set<T>().Find(id);
        }

        public List<T> GetAll<T>() where T : class
        {
            return _energyUltilityRepository.Set<T>().ToList();
        }

        public void Add<T>(T entity) where T : class
        {
            _energyUltilityRepository.Set<T>().Add(entity);
            _energyUltilityRepository.SaveChanges();
        }

        public void Update<T>(T entity) where T : class
        {
            _energyUltilityRepository.Set<T>().Update(entity);
            _energyUltilityRepository.SaveChanges();
        }
    }
}
