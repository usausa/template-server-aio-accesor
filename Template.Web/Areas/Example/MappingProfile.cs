namespace Template.Web.Areas.Example;

using Template.Web.Areas.Example.Models;

#pragma warning disable IDE0320
public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Data
        CreateMap<DataListForm, DataSearchParameter>()
            .ForMember(d => d.Page, opt => opt.MapFrom(s => s.Page ?? 1))
            .ForMember(d => d.DateTimeTo, opt => opt.MapFrom(s => s.DateTimeTo.NextDate()));
    }
}
#pragma warning restore IDE0320
