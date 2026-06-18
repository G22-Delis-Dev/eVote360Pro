# Documento de Requisitos

## Introducción

El flujo del elector en **eVote360 Pro** cubre el proceso completo que sigue un ciudadano desde que ingresa su número de documento hasta que finaliza su votación y recibe la confirmación. El sistema debe garantizar la identidad del elector mediante validación OCR de su cédula, un código de verificación de un solo uso enviado por correo, y la integridad de los votos mediante transacciones atómicas. La navegación y el acceso deben estar restringidos por rol para impedir acceso no autorizado.

---

## Glosario

- **Sistema**: La aplicación web eVote360 Pro en su conjunto.
- **Elector**: Ciudadano registrado con rol "Elector" que accede al flujo de votación.
- **Ciudadano**: Entidad registrada en el padrón electoral, representada por `CiudadanoDto`.
- **Elección_Activa**: Instancia de `EleccionDto` cuyo `Estado` es `Activa` en la fecha actual.
- **Número_Documento**: Cadena de texto alfanumérica que identifica la cédula del Ciudadano.
- **OCR_Service**: Servicio `IOcrService` que extrae el `Número_Documento` a partir de una imagen de cédula.
- **Código_Verificación**: Cadena numérica de 6 dígitos de un solo uso, válida por un período de tiempo definido.
- **Boleta_Electoral**: Conjunto de `PuestoBoletaDto` con sus `CandidatoBoletaDto` asociados, disponibles para la Elección_Activa.
- **Puesto_Electivo**: Cargo para el que se puede votar, representado por `PuestoBoletaDto`.
- **Candidato**: Postulante a un Puesto_Electivo, representado por `CandidatoBoletaDto`.
- **Opción_Ninguno**: Selección especial disponible en cada Puesto_Electivo que representa un voto en blanco para ese puesto.
- **Votación_Service**: Servicio `IVotacionService` que orquesta los pasos del flujo de votación.
- **Email_Template_Service**: Servicio `IEmailTemplateService` que genera el contenido HTML de los correos.
- **Voto**: Registro anónimo asociado a un `PuestoElectivoId` y opcionalmente a un `CandidatoId`, representado por `VotoDto`.
- **Resumen_Votación**: Objeto `ResumenVotacionDto` con la lista de `VotoResumenDto` enviado al Elector tras votar.
- **Layout_General**: Plantilla visual compartida por todos los módulos de la aplicación.
- **Rol**: Nivel de acceso asignado a un usuario autenticado (p. ej. Elector, Administrador).
- **Período_Validez_Código**: Tiempo máximo en minutos durante el cual el Código_Verificación es aceptado.

---

## Requisitos

### Requisito 1: Menú principal con navegación por rol

**User Story:** Como usuario autenticado, quiero ver únicamente las opciones de menú correspondientes a mi Rol, para que no pueda acceder a funcionalidades que no me corresponden.

#### Criterios de Aceptación

1. WHEN un usuario autenticado accede al menú principal, THE Sistema SHALL mostrar exclusivamente las opciones de navegación asociadas al Rol del usuario.
2. WHEN un usuario con Rol Elector accede al menú principal, THE Sistema SHALL incluir la opción de acceso al flujo de votación.
3. WHEN un usuario con Rol distinto de Elector accede al menú principal, THE Sistema SHALL omitir la opción del flujo de votación del Elector.

---

### Requisito 2: Navegación entre módulos y layout consistente

**User Story:** Como usuario de la aplicación, quiero que la navegación entre módulos sea coherente y que todos los módulos utilicen el mismo Layout_General, para tener una experiencia visual uniforme.

#### Criterios de Aceptación

1. THE Sistema SHALL aplicar el Layout_General en todas las pantallas del flujo del Elector.
2. WHEN el usuario navega de un módulo a otro, THE Sistema SHALL mantener el Layout_General activo sin recarga completa de la página.
3. WHEN el Elector completa o abandona el flujo de votación, THE Sistema SHALL redirigir al usuario a la pantalla de inicio del módulo correspondiente.

---

### Requisito 3: Restricción de acceso directo por URL según rol

**User Story:** Como administrador de seguridad, quiero que las rutas del flujo del Elector no sean accesibles directamente por URL sin la autorización correspondiente, para evitar acceso no autorizado.

#### Criterios de Aceptación

1. WHEN un usuario no autenticado intenta acceder directamente por URL a cualquier ruta del flujo del Elector, THE Sistema SHALL redirigir al usuario a la pantalla de inicio de sesión.
2. WHEN un usuario autenticado con Rol distinto de Elector intenta acceder directamente por URL a cualquier ruta del flujo del Elector, THE Sistema SHALL mostrar una pantalla de acceso denegado con un mensaje descriptivo.
3. WHEN un Elector intenta acceder directamente por URL a un paso intermedio del flujo de votación sin haber completado los pasos previos, THE Sistema SHALL redirigir al Elector al primer paso pendiente del flujo.

---

### Requisito 4: Pantalla inicial del Elector e ingreso del número de documento

**User Story:** Como Elector, quiero una pantalla inicial donde ingresar mi Número_Documento para comenzar el proceso de votación.

#### Criterios de Aceptación

1. THE Sistema SHALL presentar al Elector una pantalla inicial con un campo de texto para ingresar el Número_Documento.
2. WHEN el Elector ingresa el Número_Documento, THE Sistema SHALL aceptar únicamente valores de tipo texto (cadena de caracteres) en dicho campo.
3. IF el Elector envía el formulario con el campo Número_Documento vacío, THEN THE Sistema SHALL mostrar un mensaje de validación indicando que el campo es obligatorio.
4. IF el Elector envía el campo Número_Documento con caracteres no permitidos, THEN THE Sistema SHALL mostrar un mensaje de validación descriptivo sin bloquear la interfaz.

---

### Requisito 5: Validación de existencia de elección activa

**User Story:** Como Elector, quiero que el sistema verifique si existe una Elección_Activa antes de permitirme continuar, para que no inicie un proceso de votación fuera de un período electoral vigente.

#### Criterios de Aceptación

1. WHEN el Elector envía su Número_Documento, THE Votación_Service SHALL consultar si existe una Elección_Activa mediante `ObtenerEleccionActivaAsync`.
2. IF no existe ninguna Elección_Activa, THEN THE Sistema SHALL mostrar al Elector un mensaje indicando que no hay elecciones activas en este momento y no permitirá continuar el flujo.
3. WHEN existe una Elección_Activa, THE Sistema SHALL continuar al siguiente paso de validación del Ciudadano.

---

### Requisito 6: Validación del ciudadano registrado, activo y no votado

**User Story:** Como Elector, quiero que el sistema verifique que mi Número_Documento corresponde a un Ciudadano registrado, activo y que aún no ha votado en la Elección_Activa, para garantizar la integridad del padrón.

#### Criterios de Aceptación

1. WHEN el Votación_Service recibe un Número_Documento y el identificador de la Elección_Activa, THE Votación_Service SHALL invocar `ValidarCiudadanoParaVotarAsync` para verificar el estado del Ciudadano.
2. IF el Número_Documento no corresponde a ningún Ciudadano registrado, THEN THE Sistema SHALL mostrar al Elector un mensaje indicando que el documento no se encuentra en el padrón electoral.
3. IF el Ciudadano correspondiente tiene el campo `Activo` igual a `false`, THEN THE Sistema SHALL mostrar al Elector un mensaje indicando que el ciudadano no está habilitado para votar.
4. IF el Ciudadano ya registró su participación en la Elección_Activa, THEN THE Sistema SHALL mostrar al Elector un mensaje indicando que ya ejerció su voto en esta elección.
5. WHEN el Ciudadano es válido, activo y no ha votado, THE Sistema SHALL continuar al paso de validación OCR.

---

### Requisito 7: Carga de imagen de cédula y validación OCR

**User Story:** Como Elector, quiero cargar una imagen de mi cédula para que el sistema confirme que el documento físico coincide con el Número_Documento ingresado, para garantizar la autenticidad de la identidad.

#### Criterios de Aceptación

1. WHEN el Ciudadano es validado exitosamente, THE Sistema SHALL presentar al Elector una pantalla para cargar la imagen de su cédula de identidad.
2. WHEN el Elector carga la imagen de la cédula, THE Votación_Service SHALL invocar `ValidarOcrAsync` pasando la cadena del Número_Documento ingresado y el `Stream` de la imagen.
3. THE OCR_Service SHALL extraer el Número_Documento contenido en la imagen mediante `ExtraerNumeroDocumentoAsync`.
4. WHEN el Número_Documento extraído por OCR coincide con el Número_Documento ingresado por el Elector, THE Sistema SHALL continuar al paso de generación del Código_Verificación.
5. IF el Número_Documento extraído por OCR no coincide con el Número_Documento ingresado, THEN THE Sistema SHALL mostrar al Elector un mensaje indicando que la imagen de cédula no corresponde al número ingresado y no permitirá continuar.
6. IF la imagen cargada no permite extraer un Número_Documento legible, THEN THE Sistema SHALL mostrar al Elector un mensaje descriptivo indicando que la imagen no es válida para el procesamiento OCR.

---

### Requisito 8: Generación y envío del código de verificación

**User Story:** Como Elector, quiero recibir un Código_Verificación de 6 dígitos en mi correo electrónico para confirmar mi identidad antes de votar.

#### Criterios de Aceptación

1. WHEN la validación OCR es exitosa, THE Votación_Service SHALL invocar `GenerarYEnviarCodigoAsync` con el identificador del Ciudadano y el identificador de la Elección_Activa.
2. THE Votación_Service SHALL generar un Código_Verificación compuesto por exactamente 6 dígitos numéricos.
3. THE Email_Template_Service SHALL generar el contenido HTML del correo mediante `GenerarCodigoVerificacionHtml` incluyendo el nombre del Ciudadano y el Código_Verificación.
4. WHEN el Código_Verificación es generado, THE Sistema SHALL enviar el correo al `CorreoElectronico` registrado en el `CiudadanoDto` del Elector.
5. THE Sistema SHALL presentar al Elector una pantalla para ingresar el Código_Verificación recibido.

---

### Requisito 9: Validación del código de verificación

**User Story:** Como Elector, quiero que el sistema valide que el código ingresado sea correcto, vigente y no haya sido utilizado previamente, para proteger el proceso de votación.

#### Criterios de Aceptación

1. WHEN el Elector ingresa un Código_Verificación, THE Votación_Service SHALL invocar `ValidarCodigoVerificacionAsync` con el identificador del Ciudadano, el identificador de la Elección_Activa y el código ingresado.
2. WHILE el Código_Verificación se encuentra dentro del Período_Validez_Código y no ha sido utilizado, THE Sistema SHALL permitir al Elector continuar al paso de la Boleta_Electoral.
3. IF el código ingresado no coincide con el Código_Verificación generado, THEN THE Sistema SHALL mostrar al Elector un mensaje indicando que el código es incorrecto.
4. IF el Código_Verificación ha expirado según el Período_Validez_Código, THEN THE Sistema SHALL mostrar al Elector un mensaje indicando que el código ha vencido y ofrecer la opción de solicitar un nuevo código.
5. IF el Código_Verificación ya fue utilizado en la misma sesión de votación, THEN THE Sistema SHALL mostrar al Elector un mensaje indicando que el código ya fue empleado.

---

### Requisito 10: Presentación de la boleta electoral y selección de candidatos

**User Story:** Como Elector, quiero ver la Boleta_Electoral con todos los Puestos_Electivos disponibles y sus Candidatos, para poder ejercer mi voto.

#### Criterios de Aceptación

1. WHEN el Código_Verificación es validado exitosamente, THE Votación_Service SHALL invocar `ObtenerBoletaElectoralAsync` con el identificador de la Elección_Activa.
2. THE Sistema SHALL mostrar al Elector todos los Puestos_Electivos disponibles en la Elección_Activa, cada uno con su lista de Candidatos postulados.
3. THE Sistema SHALL incluir la Opción_Ninguno como selección disponible en cada Puesto_Electivo de la Boleta_Electoral.
4. WHEN el Elector selecciona un Candidato para un Puesto_Electivo, THE Sistema SHALL marcar visualmente dicha selección y permitir al Elector modificarla antes de finalizar la votación.
5. WHEN el Elector selecciona la Opción_Ninguno para un Puesto_Electivo, THE Sistema SHALL registrar la selección como voto en blanco para ese puesto y permitir al Elector modificarla antes de finalizar la votación.

---

### Requisito 11: Validación de selección obligatoria y finalización de la votación

**User Story:** Como Elector, quiero que el sistema me exija haber seleccionado una opción en cada Puesto_Electivo antes de finalizar, para evitar que mi votación quede incompleta.

#### Criterios de Aceptación

1. IF el Elector intenta finalizar la votación sin haber realizado una selección en al menos un Puesto_Electivo, THEN THE Sistema SHALL mostrar un mensaje de validación indicando los puestos sin selección y no permitirá continuar.
2. WHEN el Elector ha realizado una selección para cada Puesto_Electivo disponible en la Boleta_Electoral, THE Sistema SHALL habilitar la acción de confirmar y finalizar la votación.
3. WHEN el Elector confirma la finalización de la votación, THE Sistema SHALL mostrar una pantalla de confirmación que resume las selecciones realizadas antes de procesar el voto.
4. WHEN el Elector acepta la confirmación, THE Votación_Service SHALL invocar `ProcesarVotacionAsync` con el identificador del Ciudadano, el identificador de la Elección_Activa y la lista de `VotoDto` generada a partir de las selecciones.
5. THE Votación_Service SHALL registrar la participación del Ciudadano y persistir los Votos de forma anónima dentro de una misma transacción atómica.

---

### Requisito 12: Envío del correo de resumen de votación

**User Story:** Como Elector, quiero recibir un correo electrónico con el resumen de mi votación después de finalizar el proceso, para tener constancia de mi participación.

#### Criterios de Aceptación

1. WHEN el proceso de votación es finalizado exitosamente, THE Votación_Service SHALL invocar `EnviarNotificacionVotoAsync` con el `CorreoElectronico` del Ciudadano, su nombre y el `ResumenVotacionDto` generado.
2. THE Email_Template_Service SHALL generar el contenido HTML del correo mediante `GenerarResumenVotacionHtml` incluyendo el nombre del Ciudadano y el Resumen_Votación.
3. THE Sistema SHALL mostrar al Elector una pantalla de finalización indicando que su voto fue registrado exitosamente y que recibirá el resumen por correo.

---

### Requisito 13: Mensajes de validación y confirmación claros

**User Story:** Como Elector, quiero que todos los mensajes de error, validación y confirmación sean claros y descriptivos, para entender qué acción debo tomar en cada situación.

#### Criterios de Aceptación

1. THE Sistema SHALL mostrar mensajes de validación con texto descriptivo que identifique la causa del problema en cada etapa del flujo de votación.
2. WHEN una acción del Elector resulta en un error del servidor, THE Sistema SHALL mostrar un mensaje de error genérico sin exponer detalles técnicos internos.
3. WHEN una operación se completa exitosamente, THE Sistema SHALL mostrar un mensaje de confirmación que indique el resultado de la acción.
