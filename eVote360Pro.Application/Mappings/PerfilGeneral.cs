using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.ViewModels.Alianzas;
using eVote360Pro.Application.ViewModels.AsignacionCandidatos;
using eVote360Pro.Application.ViewModels.Candidatos;
using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Application.Mappings;

public class PerfilGeneral : Profile
{
    public PerfilGeneral()
    {
        // =========================================================
        // MAPEOS DE SEGURIDAD Y ACCESO
        // =========================================================
        CreateMap<Usuario, UsuarioDto>().ReverseMap();

        // =========================================================
        //  MAPEOS SIMPLES (Entidad <-> DTO)
        // =========================================================
        CreateMap<PartidoPolitico, PartidoPoliticoDto>().ReverseMap();
        CreateMap<AlianzaPolitica, AlianzaPoliticaDto>().ReverseMap();
        CreateMap<PuestoElectivo, PuestoElectivoDto>().ReverseMap();

        // =========================================================
        // MAPEOS COMPLEJOS (Con lógica o relaciones)
        // =========================================================
        CreateMap<Candidato, CandidatoDto>()
            .ForMember(dest => dest.NombrePartido, opt => opt.MapFrom(src => src.PartidoPolitico.Nombre))
            .ForMember(dest => dest.LogoPartido, opt => opt.MapFrom(src => src.PartidoPolitico.LogoRuta))
            .ReverseMap();

        CreateMap<AsignacionCandidatoPuesto, AsignacionCandidatoPuestoDto>()
            .ForMember(dest => dest.CandidatoNombreCompleto, opt => opt.MapFrom(src => $"{src.Candidato.Nombre} {src.Candidato.Apellido}"))
            .ForMember(dest => dest.PuestoNombre, opt => opt.MapFrom(src => src.PuestoElectivo.Nombre))
            .ForMember(dest => dest.PartidoNombre, opt => opt.MapFrom(src => src.PartidoPolitico.Nombre))
            .ReverseMap();

        // =========================================================
        // MAPEOS DE VISTAS Y FORMULARIOS (ViewModels <-> DTO)
        // =========================================================

        // --- Módulo: Alianzas Políticas ---
        CreateMap<AlianzaPoliticaDto, AlianzaListViewModel>();
        CreateMap<AlianzaCreateViewModel, AlianzaPoliticaDto>().ReverseMap();

        // --- Módulo: Candidatos ---
        CreateMap<CandidatoDto, CandidatoListViewModel>()
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => $"{src.Nombre} {src.Apellido}"))
            .ForMember(dest => dest.PartidoPoliticoNombre, opt => opt.MapFrom(src => src.NombrePartido));
        CreateMap<CandidatoCreateViewModel, CandidatoDto>().ReverseMap();
        CreateMap<CandidatoEditViewModel, CandidatoDto>().ReverseMap();

        // --- Módulo: Asignación de Candidatos ---
        CreateMap<AsignacionCandidatoPuestoDto, AsignacionCandidatoListViewModel>();
        CreateMap<AsignacionCandidatoCreateViewModel, AsignacionCandidatoPuestoDto>().ReverseMap();
        CreateMap<AsignacionCandidatoEditViewModel, AsignacionCandidatoPuestoDto>().ReverseMap();
    }
}