using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Interfaces;

public interface IVotacionService
{
    // Paso 1: Verificar si hay una elección activa hoy
    Task<EleccionDto?> ObtenerEleccionActivaAsync();

    // Paso 2: Validar si la cédula existe, está activa y NO ha votado en esta elección
    Task<CiudadanoDto> ValidarCiudadanoParaVotarAsync(string cedula, int eleccionId);

    // Paso 3: Generar un código de 6 dígitos y enviar por correo
    Task GenerarYEnviarCodigoAsync(int ciudadanoId, int eleccionId);

    // Paso 4: Validar que el código que ingresó el usuario es correcto y no ha expirado
    Task<bool> ValidarCodigoVerificacionAsync(int ciudadanoId, int eleccionId, string codigo);

    // Paso 5: Obtener la boleta electoral (los puestos y sus candidatos) para mostrar en pantalla
    Task<IEnumerable<PuestoBoletaDto>> ObtenerBoletaElectoralAsync(int eleccionId);

    // Paso 6: Guardar los votos de forma anónima y registrar que el ciudadano ya participó (Esto lo hacemos usando transacciones)
    Task ProcesarVotacionAsync(int ciudadanoId, int eleccionId, IEnumerable<VotoDto> votos);

    // Paso 7: Enviar notificación con el resumen de votación al ciudadano por correo
    Task EnviarNotificacionVotoAsync(string email, string nombre, ResumenVotacionDto resumen);
}