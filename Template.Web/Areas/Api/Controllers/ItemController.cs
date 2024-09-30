namespace Template.Web.Areas.Api.Controllers;

using Template.Web.Areas.Api.Models;

public sealed class ItemController : BaseApiController
{
    private IMapper Mapper { get; }

    private ItemService ItemService { get; }

    public ItemController(
        IMapper mapper,
        ItemService itemService)
    {
        Mapper = mapper;
        ItemService = itemService;
    }

    [HttpGet("{category}")]
    public async ValueTask<IActionResult> List([FromRoute] string category)
    {
        return Ok(new ItemListResponse
        {
            Entries = [.. await ItemService.QueryItemListAsync(category)]
        });
    }

    [HttpPost]
    public async ValueTask<IActionResult> Update([FromBody] ItemUpdateRequest request)
    {
        await ItemService.UpdateItemList(request.Entries.Select(Mapper.Map<ItemUpdateRequestEntry, ItemEntity>));

        return Ok();
    }
}
