# eVote360 Pro

Sistema de votación electrónica académico desarrollado en ASP.NET Core MVC (.NET 9) siguiendo Onion Architecture, principios SOLID y Clean Code.

---

## Tecnologías

- **Framework:** ASP.NET Core MVC (.NET 9) con Razor Views
- **ORM:** Entity Framework Core 9 — Code First
- **Base de datos:** SQL Server
- **CSS:** Tailwind CSS
- **Email:** MailKit / MimeKit
- **OCR:** Tesseract
- **Hash de contraseñas:** BCrypt.Net-Next

---

## Arquitectura

El proyecto sigue **Onion Architecture** con las siguientes capas:

Domain → Application → Infrastructure → Web
↑
Shared

Las dependencias siempre apuntan hacia adentro. Domain no depende de ninguna capa externa.

---

## Estructura del proyecto
eVote360Pro/
├── eVote360Pro.Domain          ← Entidades, Enums, Interfaces, Excepciones, Reglas
├── eVote360Pro.Application     ← Servicios, DTOs, ViewModels
├── eVote360Pro.Infrastructure  ← EF Core, Repositorios, OcrService
├── eVote360Pro.Shared          ← EmailService, EmailSettings
└── eVote360Pro.Web             ← Controllers, Views, Tailwind
---

## Capas

### Domain
Núcleo del sistema. Sin dependencias externas.

- `Entities/` — 14 entidades del negocio
- `Enums/` — EstadoEleccion, RolUsuario, EstadoAlianza
- `Interfaces/` — IEmailService, IOcrService, IRepository\<T\>, IUnitOfWork
- `Interfaces/Repositories/` — 13 interfaces de repositorios específicos
- `Exceptions/` — 10 excepciones de negocio personalizadas
- `Rules/` — 7 clases estáticas con reglas de negocio puras

### Application
Orquesta los casos de uso. Depende solo de Domain.

- `Services/` — 10 servicios de negocio
- `Interfaces/` — Contratos de los servicios
- `DTOs/` — Objetos de transferencia de datos
- `ViewModels/` — Modelos para las vistas organizados por módulo

### Infrastructure
Implementaciones técnicas. Depende de Domain.

- `Data/AppDbContext.cs` — Contexto de EF Core
- `Data/Configurations/` — 13 clases IEntityTypeConfiguration
- `Repositories/` — Implementaciones de repositorios
- `UnitOfWork/` — Unidad de trabajo
- `Services/OcrService.cs` — Procesamiento OCR de cédulas

### Shared
Servicios transversales. Sin dependencias de otras capas del proyecto.

- `Services/EmailService.cs` — Envío de correos con MailKit
- `Services/EmailSettings.cs` — Configuración SMTP

### Web
Capa de presentación MVC. Depende de Application, Infrastructure y Shared.

- `Controllers/` — Auth, Admin, Dirigente, Elector
- `Views/` — Razor Views con Tailwind CSS
- `wwwroot/` — CSS compilado, JS, imágenes

---

## Roles del sistema

| Rol | Descripción |
|---|---|
| **Administrador** | Gestiona ciudadanos, usuarios, partidos, puestos, elecciones |
| **Dirigente Político** | Gestiona candidatos, alianzas y asignaciones de su partido |
| **Elector** | Accede mediante número de documento, vota mediante OCR + código de verificación |

---

## Módulos

### Administrador
- Autenticación y control de acceso por rol
- Home con resumen electoral por año
- Mantenimiento de Ciudadanos
- Mantenimiento de Usuarios
- Mantenimiento de Partidos Políticos
- Mantenimiento de Puestos Electivos
- Asignación de Dirigentes Políticos
- Gestión de Elecciones (Pendiente → Activa → Finalizada)

### Dirigente Político
- Home con indicadores del partido
- Mantenimiento de Candidatos
- Alianzas Políticas (solicitar, aceptar, rechazar, eliminar)
- Asignación de Candidatos a Puestos

### Elector
1. Ingreso de número de documento
2. Validación de elección activa, ciudadano registrado y activo
3. Carga de imagen de cédula para validación OCR
4. Generación y envío de código de verificación por correo
5. Validación del código de 6 dígitos
6. Selección de candidatos por puesto electivo
7. Confirmación y finalización del voto
8. Envío de resumen de votación por correo

---

## Principios aplicados

### SOLID
- **S** — Cada clase tiene una sola responsabilidad
- **O** — Abierto para extensión, cerrado para modificación
- **L** — Los repositorios específicos son sustituibles por IRepository\<T\>
- **I** — Interfaces segregadas por entidad y por servicio
- **D** — Application depende de interfaces, no de implementaciones concretas

### Clean Code
- Nombres descriptivos en español consistentes con el dominio
- Métodos pequeños con una sola responsabilidad
- Sin números mágicos — enums reemplazan valores hardcodeados
- Un archivo por clase

### Onion Architecture
- Domain sin dependencias externas
- Las dependencias apuntan siempre hacia adentro
- Infrastructure implementa interfaces de Domain
- ViewModels en Application para soportar múltiples presentaciones futuras

---

## Seguridad

- Contraseñas hasheadas con BCrypt
- Control de acceso por rol en cada controlador
- Confidencialidad del voto — la tabla `Voto` no almacena `CiudadanoId`
- `ParticipacionElectoral` separa "¿ya votó?" del voto en sí
- Protección contra acceso directo por URL según rol
- Códigos de verificación de un solo uso con expiración de 5 minutos

---

## Configuración

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=eVote360Pro;Trusted_Connection=True;"
  },
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Puerto": 587,
    "CorreoRemitente": "tucorreo@gmail.com",
    "NombreRemitente": "eVote360 Pro",
    "Password": "tu_app_password"
  }
}
```

### Tesseract
Descarga el archivo de idioma español y colócalo en:
eVote360Pro.Web/
└── tessdata/
└── spa.traineddata

Propiedad del archivo en VS: **Copy to Output Directory → Copy if newer**

### Tailwind CSS
```bash
npm install
npm run build:css
```

---

## Migraciones

```powershell
# Crear migración inicial
Add-Migration InitialCreate -Project eVote360Pro.Infrastructure -StartupProject eVote360Pro.Web

# Aplicar migración
Update-Database -Project eVote360Pro.Infrastructure -StartupProject eVote360Pro.Web
```

---

## Equipo

| Integrante | Matricula |
|---|---|
| **Delis Manuel De La Cruz Castillo** | 2025-1074 |
| **Sky Luisahanie Andujar Victorino** | 2025-1063 |
