using AutoMapper;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Mappings;

public class PerfilGeneral : Profile
{
    public PerfilGeneral()
    {
        CreateMap<Usuario, UsuarioDto>().ReverseMap();
        CreateMap<PartidoPolitico, PartidoPoliticoDto>().ReverseMap();
        CreateMap<Candidato, CandidatoDto>()
     .ForMember(dest => dest.NombrePartido, opt => opt.MapFrom(src => src.PartidoPolitico.Nombre))
     .ForMember(dest => dest.LogoPartido, opt => opt.MapFrom(src => src.PartidoPolitico.LogoRuta)) // O el nombre de propiedad que tenga el logo en tu entidad PartidoPolitico
     .ReverseMap();
    }
}