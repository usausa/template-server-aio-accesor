namespace Template.Web.Areas.Api;

using Template.Web.Areas.Api.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ItemUpdateRequestEntry, ItemEntity>();
    }
}
