using AutoMapper;
using WebApiPIA.DTOs;
using WebApiPIA.Entidades;

namespace WebApiPIA.Utilidades
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<RifaCreacionDTO, Rifa>();
            CreateMap<ClienteCreacionDTO, Cliente>();
            CreateMap<BoletoCreacionDTO, Boleto>();
            CreateMap<PremioCreacionDTO, Premio>();

            CreateMap<Cliente,GetClienteDTO>();
            CreateMap<Premio, GetPremioDTO>();
            CreateMap<Rifa, GetRifaDTO>();
            CreateMap<Boleto, GetBoletoDTO>();

            CreateMap<ClientePatchDTO, Cliente>().ReverseMap();
            CreateMap<BoletoPatchDTO, Boleto>().ReverseMap();
            CreateMap<RifaPatchDTO, Rifa>().ReverseMap();
            CreateMap<PremioPatchDTO, Premio>().ReverseMap();
        }
    }
}
