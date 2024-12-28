using Microsoft.AspNetCore.Mvc;
using Tahyour.Base.Common.Domain.Common;
using Tahyour.Base.Common.Presentation;
using Tahyour.Base.Common.Repositories.Implementations;
using Tahyour.Base.Common.Services.Interface;

namespace TestApi.Controllers
{

    public class ItemsController : MongoBaseController<Item, ItemDTO>
    {
        public ItemsController(IMongoBaseService<Item> service) : base(service) { }

        [HttpPost]
        public async Task<Result<ItemDTO>> CreateAsync([FromBody] ItemDTO request)
        {
            return await CreateAsync<ItemDTO>(request);
        }

        [HttpPut("{id}")]
        public async Task<Result<bool>> UpdateAsync(Guid id, [FromBody] ItemDTO request)
        {
            return await UpdateAsync<ItemDTO>(id, request);
        }
    }

}
