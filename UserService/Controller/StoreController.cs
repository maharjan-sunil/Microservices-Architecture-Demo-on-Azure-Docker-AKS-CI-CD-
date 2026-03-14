using DockerDemo.Docker.Interface;
using DockerDemo.Docker.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DockerDemo.Docker.Controller
{
   [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {

        private readonly IStoreService _service;
        public StoreController(IStoreService service)
        {
            _service = service;
        }

        [HttpGet]
        public  async Task<ActionResult<IEnumerable<Store>>> GetStores()
        {
            return await _service.GetStores();
        }
       
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddStore(Store store)
        {
            await _service.AddStore(store);
            return Ok();
                
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> UpdateStore(Store store)
        {
            await _service.UpdateStore(store);
            return Ok();

        }

        [HttpDelete]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteStore(int id)
        {
            await _service.DeleteStore(id);
            return Ok();

        }

       
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Store>>> GetStoreByName(String storeName)
        {
            return await _service.GetStoreByName(storeName);

        }
    }
}

