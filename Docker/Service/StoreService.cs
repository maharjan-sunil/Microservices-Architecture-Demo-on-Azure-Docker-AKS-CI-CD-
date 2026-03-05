using DockerDemo.Data;
using DockerDemo.Docker.Interface;
using DockerDemo.Docker.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DockerDemo.Docker.Service
{
    public class StoreService : IStoreService
    {

        private readonly ApplicationDbContext _context;

        public StoreService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddStore(Store store)
        {
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStore(int id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store != null)
            {
                _context.Stores.Remove(store);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ActionResult<IEnumerable<Store>>> GetStores()
        {
            return await _context.Stores.ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<Store>>> GetStoreByName(string storeName)
        {
            IEnumerable<Store> filteredStores;
            if (string.IsNullOrEmpty(storeName))
            {
                return await _context.Stores.ToListAsync();
            }

            // Use Where() with Contains() for 'like' functionality
            // This translates to a SQL LIKE '%...%' query
            return await _context.Stores
                .Where(s => s.Name.Contains(storeName))
                .ToListAsync();
        }

        public Task UpdateStore(Store store)
        {
            throw new NotImplementedException();
        }

    }
}
