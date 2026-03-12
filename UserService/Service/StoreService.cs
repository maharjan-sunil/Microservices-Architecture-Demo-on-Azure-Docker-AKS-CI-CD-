using DockerDemo.Data;
using DockerDemo.Docker.Interface;
using DockerDemo.Docker.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

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

        public async Task<ActionResult<IEnumerable<Store>>> UpdateStore(Store store)
        {


            var storeDetail = await _context.Stores.FindAsync(store.Id);

            // if (storeDetail == null)
            // {
            //   //  return NotFound($"Store with ID {store.Id} not found.");
            // }


            storeDetail.Name = store.Name;
            storeDetail.Email = store.Email;


            // // Mark the entity as modified and save
            // _context.Entry(store).State = EntityState.Modified;

            //// try
            // //{
            await _context.SaveChangesAsync();
            return await _context.Stores
                .Where(s => s.Id.Equals(store.Id))
                .ToListAsync();
            // return OK(storeDetail);
            // //}
            // //catch (DbUpdateConcurrencyException)
            // // {
            // //    return Conflict("The store was updated by another user.");
            // //}

        }

        Task IStoreService.UpdateStore(Store store)
        {
            return UpdateStore(store);
        }
    }
}
