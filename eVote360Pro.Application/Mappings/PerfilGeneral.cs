using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.ViewModels.Alianzas;
using eVote360Pro.Application.ViewModels.Candidatos;
using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Application.Mappings;

public class PerfilGeneral : Profile
{
    public PerfilGeneral()
    {
        // =========================================================
        // 1. MAPEOS DE SEGURIDAD Y ACCESO
        // =========================================================
        CreateMap<Usuario, UsuarioDto>().ReverseMap();

        // =========================================================
        // 2. MAPEOS SIMPLES (Entidad <-> DTO)
        // =========================================================
        CreateMap<PartidoPolitico, PartidoPoliticoDto>().ReverseMap();
        CreateMap<AlianzaPolitica, AlianzaPoliticaDto>().ReverseMap();

        // =========================================================
        // 3. MAPEOS COMPLEJOS (Con lógica o relaciones)
        // =========================================================
        CreateMap<Candidato, CandidatoDto>()
            .ForMember(dest => dest.NombrePartido, opt => opt.MapFrom(src => src.PartidoPolitico.Nombre))
            .ForMember(dest => dest.LogoPartido, opt => opt.MapFrom(src => src.PartidoPolitico.LogoRuta))
            .ReverseMap();

        // =========================================================
        // 4. MAPEOS DE VISTAS Y FORMULARIOS (ViewModels <-> DTO)
        // =========================================================
        CreateMap<AlianzaPoliticaCreateViewModel, AlianzaPoliticaDto>().ReverseMap();
        CreateMap<CandidatoCreateViewModel, CandidatoDto>().ReverseMap();


        // Mapeo para la tabla de candidatos (Index)
        CreateMap<CandidatoDto, CandidatoListViewModel>()
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => $"{src.Nombre} {src.Apellido}"))
            .ForMember(dest => dest.PartidoPoliticoNombre, opt => opt.MapFrom(src => src.NombrePartido));

        // Mapeo bidireccional para el formulario de edición de candidatos
        CreateMap<CandidatoEditViewModel, CandidatoDto>().ReverseMap();

        // Mapeo para la tabla de alianzas (Index)
        CreateMap<AlianzaPoliticaDto, AlianzaListViewModel>();
    }
}