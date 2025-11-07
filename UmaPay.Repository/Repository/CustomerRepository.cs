using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace UmaPay.Repository
{
    using Interface.Repository;
    using UmaPay.Domain;

    public class CustomerRepository : GenericRepository<Entities.Customer>, ICustomerQueryRepository, ICustomerCommandRepository
    {
        private readonly IMapper _mapper;

        public CustomerRepository(DbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #region Query Methods

        public async Task<Customer> GetByIdAsync(int id)
        {
            var entity = await GetAsync(id);
            return _mapper.Map<Customer>(entity);
        }

        public async Task<Customer> GetByEmailAsync(string email)
        {
            var entity = await FindAsync(c => c.Email == email);
            return _mapper.Map<Customer>(entity);
        }

        public async Task<Customer> GetByCodeSapAsync(string codeSap)
        {
            try
            {
                var entity = await FindAsync(c => c.CodeSap == codeSap);
                return _mapper.Map<Customer>(entity);
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            var entities = await GetAllAsyn();
            return _mapper.Map<IEnumerable<Customer>>(entities);
        }

        #endregion

        #region Command Methods

        public async Task<Customer> AddAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var entity = _mapper.Map<Entities.Customer>(customer);
            var addedEntity = await AddAsyn(entity);
            return _mapper.Map<Customer>(addedEntity);
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var entity = _mapper.Map<Entities.Customer>(customer);
            var updatedEntity = await UpdateAsync(entity, entity.CustormerId);
            return _mapper.Map<Customer>(updatedEntity);
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