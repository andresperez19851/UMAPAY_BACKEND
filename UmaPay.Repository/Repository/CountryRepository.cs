using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace UmaPay.Repository
{
    using Interface.Repository;
    using UmaPay.Domain;

    public class CountryRepository : GenericRepository<Entities.Country>, ICountryQueryRepository, ICountryCommandRepository
    {
        private readonly IMapper _mapper;

        public CountryRepository(DbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #region Query Methods

        public async Task<Country> GetByIdAsync(int id)
        {
            var entity = await GetAsync(id);
            return _mapper.Map<Country>(entity);
        }

        public async Task<Country> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

            var entity = await FindAsync(c => c.Name == name);
            return _mapper.Map<Country>(entity);
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            var entities = await GetAllAsyn();
            return _mapper.Map<IEnumerable<Country>>(entities);
        }

        #endregion

        #region Command Methods

        public async Task<Country> AddAsync(Country country)
        {
            if (country == null)
                throw new ArgumentNullException(nameof(country));

            var entity = _mapper.Map<Entities.Country>(country);
            var addedEntity = await AddAsyn(entity);
            return _mapper.Map<Country>(addedEntity);
        }

        public async Task<Country> UpdateAsync(Country country)
        {
            if (country == null)
                throw new ArgumentNullException(nameof(country));

            var entity = _mapper.Map<Entities.Country>(country);
            var updatedEntity = await UpdateAsync(entity, entity.CountryId);
            return _mapper.Map<Country>(updatedEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            if (entity != null)
            {
                await DeleteAsync(entity);
            }
        }

        #endregion
    }
}