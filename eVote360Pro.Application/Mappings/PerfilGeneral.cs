using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.ViewModels.Alianzas;
using eVote360Pro.Application.ViewModels.AsignacionCandidatos;
using eVote360Pro.Application.ViewModels.AsignacionDirigentes;
using eVote360Pro.Application.ViewModels.Candidatos;
using eVote360Pro.Application.ViewModels.Ciudadanos;
using eVote360Pro.Application.ViewModels.Elecciones;
using eVote360Pro.Application.ViewModels.Partidos;
using eVote360Pro.Application.ViewModels.PuestosElectivos;
using eVote360Pro.Application.ViewModels.Usuarios;
using eVote360Pro.Application.ViewModels.Votacion;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Enums;

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
        CreateMap<Ciudadano, CiudadanoDto>().ReverseMap();
        CreateMap<Eleccion, EleccionDto>().ReverseMap();
        CreateMap<AsignacionDirigente, AsignacionDirigenteDto>()
            .ForMember(dest => dest.NombreDirigente, opt => opt.MapFrom(src => $"{src.Usuario.Nombre} {src.Usuario.Apellido}"))
            .ForMember(dest => dest.NombrePartido, opt => opt.MapFrom(src => src.PartidoPolitico.Nombre))
            .ForMember(dest => dest.SiglaPartido, opt => opt.MapFrom(src => src.PartidoPolitico.Siglas));
        CreateMap<AsignacionDirigenteDto, AsignacionDirigente>();

        // =========================================================
        // MAPEOS COMPLEJOS (Con lógica o relaciones)
        // =========================================================
        CreateMap<Candidato, CandidatoDto>()
            .ForMember(dest => dest.NombrePartido, opt => opt.MapFrom(src => src.PartidoPolitico.Nombre))
            .ForMember(dest => dest.LogoPartido, opt => opt.MapFrom(src => src.PartidoPolitico.LogoRuta));
        CreateMap<CandidatoDto, Candidato>();

        CreateMap<AsignacionCandidatoPuesto, AsignacionCandidatoPuestoDto>()
            .ForMember(dest => dest.CandidatoNombreCompleto, opt => opt.MapFrom(src => $"{src.Candidato.Nombre} {src.Candidato.Apellido}"))
            .ForMember(dest => dest.PuestoNombre, opt => opt.MapFrom(src => src.PuestoElectivo.Nombre))
            .ForMember(dest => dest.PartidoNombre, opt => opt.MapFrom(src => src.PartidoPolitico.Nombre));
        CreateMap<AsignacionCandidatoPuestoDto, AsignacionCandidatoPuesto>();

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

        // --- Módulo: Proceso de Votación (Ciudadano) ---
        CreateMap<VotoDto, SeleccionVotoViewModel>().ReverseMap();

        // esto es opcional, pero lo puse para que no haya problemas si pasan objetos complejos
        CreateMap<EleccionDto, InicioVotacionViewModel>().ReverseMap();

        // =========================================================
        // MAPEOS DE VISTAS ADMIN (ViewModels <-> DTO)
        // =========================================================

        // --- Módulo: Ciudadanos (Admin) ---
        CreateMap<CiudadanoDto, CiudadanoItemViewModel>()
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => $"{src.Nombre} {src.Apellido}"));
        CreateMap<CiudadanoCreateViewModel, CiudadanoDto>().ReverseMap();
        CreateMap<CiudadanoEditViewModel, CiudadanoDto>().ReverseMap();

        // --- Módulo: Partidos Políticos (Admin) ---
        CreateMap<PartidoPoliticoDto, PartidoItemViewModel>();
        CreateMap<PartidoCreateViewModel, PartidoPoliticoDto>().ReverseMap();
        CreateMap<PartidoEditViewModel, PartidoPoliticoDto>()
            .ForMember(dest => dest.LogoRuta, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.LogoActualRuta, opt => opt.MapFrom(src => src.LogoRuta));

        // --- Módulo: Puestos Electivos (Admin) ---
        CreateMap<PuestoElectivoDto, PuestoElectivoItemViewModel>();
        CreateMap<PuestoElectivoCreateViewModel, PuestoElectivoDto>().ReverseMap();
        CreateMap<PuestoElectivoEditViewModel, PuestoElectivoDto>().ReverseMap();

        // --- Módulo: Usuarios (Admin) ---
        CreateMap<UsuarioDto, UsuarioItemViewModel>()
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => $"{src.Nombre} {src.Apellido}"))
            .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.Rol == RolUsuario.Administrador ? "Administrador" : "Dirigente Político"));
        CreateMap<UsuarioCreateViewModel, UsuarioDto>()
            .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => (RolUsuario)src.Rol))
            .ReverseMap()
            .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => (int)src.Rol));
        CreateMap<UsuarioEditViewModel, UsuarioDto>()
            .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => (RolUsuario)src.Rol))
            .ReverseMap()
            .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => (int)src.Rol));

        // --- Módulo: Asignación de Dirigentes (Admin) ---
        CreateMap<AsignacionDirigenteDto, AsignacionDirigenteItemViewModel>();
        CreateMap<AsignacionDirigenteCreateViewModel, AsignacionDirigenteDto>().ReverseMap();

        // --- Módulo: Elecciones (Admin) ---
        CreateMap<EleccionDto, EleccionItemViewModel>();
        CreateMap<EleccionCreateViewModel, EleccionDto>().ReverseMap();
    }
}
