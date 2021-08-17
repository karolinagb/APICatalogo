using AutoMapper;

namespace APICatalogo.Models.ViewModels.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Produto, ProdutoViewModel>()
                .ReverseMap()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                    .ForPath(x => x.DataCadastro, option => option.Ignore());
        }
    }
}
