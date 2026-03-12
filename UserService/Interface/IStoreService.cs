using DockerDemo.Docker.Model;
using Microsoft.AspNetCore.Mvc;

namespace DockerDemo.Docker.Interface
{
    public interface IStoreService
    {
        public Task<ActionResult<IEnumerable<Store>>> GetStores();
        public Task AddStore(Store store);
        public Task DeleteStore(int id);
        public Task<ActionResult<IEnumerable<Store>>> GetStoreByName(string storeName);
        public Task UpdateStore(Store store);
    }
}
