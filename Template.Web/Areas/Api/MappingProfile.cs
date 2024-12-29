namespace Template.Web.Areas.Api;

using Template.Web.Areas.Api.Models;

#pragma warning disable IDE0320
public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ItemUpdateRequestEntry, ItemEntity>();
    }
}
#pragma warning restore IDE0320
