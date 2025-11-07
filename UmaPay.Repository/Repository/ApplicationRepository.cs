using AutoMapper;
using Microsoft.EntityFrameworkCore;


namespace UmaPay.Repository
{
    using Interface.Repository;
    using Domain;

    public class ApplicationRepository : GenericRepository<Entities.Application>, IApplicationQueryRepository, IApplicationCommandRepository
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public ApplicationRepository(DataContext context, IMapper mapper) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #region Query Methods

        public async Task<Application> GetByIdAsync(int id)
        {
            var entity = await GetAsync(id);
            return _mapper.Map<Application>(entity);
        }

        public async Task<Application> GetByNameAsync(string name)
        {
            var entity = await FindAsync(a => a.Name == name);
            return _mapper.Map<Application>(entity);
        }

        public async Task<Application> GetByApiKeyAsync(string apiKey)
        {
            var entity = await FindAsync(a => a.ApiKey == apiKey);
            return _mapper.Map<Application>(entity);
        }

        public async Task<IEnumerable<Application>> GetAllAsync()
        {
            var entities = await GetAllAsyn();
            return _mapper.Map<IEnumerable<Application>>(entities);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Applications.AnyAsync(a => a.Name == name);
        }

        #endregion

        #region Command Methods

        public async Task<Application> AddAsync(Application command)
        {
            var entity = _mapper.Map<Entities.Application>(command);
            var addedEntity = await AddAsyn(entity);
            return _mapper.Map<Application>(addedEntity);
        }

        public async Task<Application> UpdateAsync(Application command)
        {
            var entity = _mapper.Map<Entities.Application>(command);
            var updatedEntity = await UpdateAsync(entity, entity.ApplicationId);
            return _mapper.Map<Application>(updatedEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            await DeleteAsync(entity);
        }

        #endregion
    }
}