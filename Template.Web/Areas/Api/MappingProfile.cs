namespace Template.Web.Areas.Api;

using Template.Web.Areas.Api.Models;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ItemUpdateRequestEntry, ItemEntity>();
    }
}
