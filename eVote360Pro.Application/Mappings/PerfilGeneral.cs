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
    }
}