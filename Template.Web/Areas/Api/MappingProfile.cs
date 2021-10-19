namespace Template.Web.Areas.Api
{
    using AutoMapper;

    using Template.Models.Entity;
    using Template.Web.Areas.Api.Models;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ItemUpdateRequestEntry, ItemEntity>();
        }
    }
}
