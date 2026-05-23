README - API de Mercado de Publicaciones
Descripción

Este proyecto proporciona una API para gestionar el registro, login, y la creación, actualización, visualización y eliminación de publicaciones en un marketplace. También incluye la gestión de categorías, lo que permite a los administradores crear nuevas categorías para clasificar productos. La autenticación se realiza mediante tokens generados al hacer login.

Cassandra Endpoints
1. GET Mssg
Obtiene un mensaje específico dentro de una conversación, validando que el usuario autenticado tenga permiso para visualizarlo.

📌Endpoint
GET /{mensajeId}?fechaEnvio={fechaEnvio}&conversacionId={conversacionId}

🔐Autenticación
Requiere usuario autenticado.
El usuario debe ser remitente o destinatario del mensaje.

2. PATCH MarkAsRead
Marca un mensaje como leído, validando que el usuario autenticado sea el destinatario.

📌Endpoint
PATCH /marcar-leido/{mensajeId}?fechaEnvio={fechaEnvio}&conversacionId={conversacionId}

🔐 Autenticación
Requiere usuario autenticado.
Solo el destinatario puede marcar el mensaje como leído.

3. POST EnviarMensajes
Permite enviar un mensaje dentro de una conversación.
Si no se proporciona ConversacionId, el sistema lo genera automáticamente.

📌 Endpoint
POST /

🔐 Autenticación
Requiere usuario autenticado.
El remitente se obtiene automáticamente desde el token (ClaimTypes.NameIdentifier).

📥 Body (JSON)
{
  "conversacionId": "string (opcional)",
  "destinatarioId": "string (requerido si no hay conversacionId)",
  "publicacionId": "string (requerido si no hay conversacionId)",
  "contenido": "string"
}

4. POST HistorialLoginByUser
Este endpoint permite consultar el historial de accesos de un usuario, incluyendo información relevante como fecha, IP y dispositivo utilizado.
Se utiliza principalmente para: Auditoría de seguridad. Monitoreo de actividad de usuario. Detección de accesos sospechosos

📌 Endpoint
POST /{userId}

🔐 Autenticación
Requiere usuario autenticado (con rol de admin).

🏗️ Flujo interno
- Se recibe el userId desde la URL
- Se consulta el servicio _historiales
- Se obtienen los registros asociados al usuario
- Se valida si existen resultados
- Se retorna la lista o una respuesta vacía

5. GET MensajesPorConversación
Obtiene todos los mensajes asociados a una conversación específica.

📌 Endpoint
GET /conversacion/{conversacionId}

🔐 Autenticación
Requiere usuario autenticado.

📥 Parámetros
| Nombre         | Tipo   | Requerido | Descripción           |
| -------------- | ------ | --------- | --------------------- |
| conversacionId | string | ✔️        | ID de la conversación |

6. GET ConversacionesByUser
Este endpoint permite listar todas las conversaciones en las que el usuario ha participado, sin necesidad de conocer previamente los IDs.

📌 Endpoint
GET /mis-conversaciones

🔐 Autenticación
Requiere usuario autenticado.
El usuarioId se obtiene desde el token.

📦 Respuesta Exitosa
{
  "user1-user2-pub123",
  "user1-user3-pub456",
  "user1-user4-pub789"
}


Endpoints Generales - MongoDB
1. Controlador de Registro

POST /api/auth/register

Este endpoint permite registrar un nuevo usuario. Espera los siguientes parámetros:

Solicitud:

{
    "name": "Roberto",
    "lastName": "Girasoles",
    "email": "imosa253@gmail.com",
    "password": "1234!"
}

Parámetros esperados:

name: Nombre del usuario.
lastName: Apellido del usuario.
email: Correo electrónico del usuario (Debe ser único, no puede repetirse en la base de datos).
password: Contraseña del usuario.

Respuesta:

Si el correo electrónico ya existe en la base de datos, se devuelve un error.
Si el registro es exitoso, el usuario será registrado en el sistema.
2. Controlador de Login

POST /api/auth/login

Este endpoint permite a los usuarios registrarse e iniciar sesión en el sistema para obtener un token que les permitirá acceder a otras funcionalidades (como ver y crear publicaciones).

Solicitud:

{
    "email": "imosa253@gmail.com",
    "password": "1234!"
}

Parámetros esperados:

email: Correo electrónico del usuario.
password: Contraseña del usuario.

Respuesta:

Si las credenciales son correctas, se genera un token que el usuario puede usar para autenticarse en otros endpoints.
Si las credenciales son inválidas, se devuelve un mensaje de error.
3. Controlador de Publicaciones
Crear Publicaciones

POST /api/publicacion/crear

Este endpoint permite a un usuario autenticado crear una nueva publicación en el marketplace. Requiere un token de autorización obtenido en el login.

Solicitud:

{
    "title": "Iphone",
    "description": "Telefono ha tenido un solo propietario, bien cuidado",
    "price": 400,
    "categoryId": "Tecnología",
    "tags": ["iphone 15", "iphone", "iphone usado"],
    "attributes": {
        "brand": "Apple",
        "model": "Pro Max"
    }
}

Parámetros esperados:

title: Título del producto.
description: Descripción del producto.
price: Precio del producto.
categoryId: ID de la categoría a la que pertenece el producto.
tags: Lista de etiquetas relacionadas con el producto.
attributes: Atributos adicionales del producto (por ejemplo, marca, modelo).

Respuesta:

Si la publicación se crea con éxito, se devuelve una confirmación.
Si hay un error, se devuelve un mensaje de error detallado.
Ver Mis Publicaciones

GET /api/publicacion/publicaciones

Este endpoint permite a un usuario autenticado ver todas sus publicaciones.

Parámetros esperados:

Autorización: El token generado en el login debe enviarse en el encabezado de la solicitud.

Respuesta:

Devuelve una lista de las publicaciones creadas por el usuario autenticado.
Eliminar Publicaciones

DELETE /api/publicacion/eliminar/{id}

Este endpoint permite a un usuario eliminar una publicación previamente creada.

Parámetros esperados:

id: El ID de la publicación que se desea eliminar.

Respuesta:

Si la publicación se elimina con éxito, se devuelve un mensaje de éxito.
Si la publicación no existe o el usuario no tiene permisos para eliminarla, se devuelve un mensaje de error.
Actualizar Publicaciones

PUT /api/publicacion/actualizar/{id}

Este endpoint permite a un usuario autenticado actualizar los datos de una publicación específica.

Solicitud:

{
    "title": "Iphone 14",
    "price": 399,
    "description": "Es un iphone bien cuidado",
    "tags": ["iphone", "nuevo"],
    "condition": "Usado"
}

Parámetros esperados:

id: ID de la publicación que se desea actualizar.
title: Nuevo título de la publicación.
price: Nuevo precio de la publicación.
description: Nueva descripción de la publicación.
tags: Nuevas etiquetas de la publicación.
condition: Nueva condición del producto (ej. nuevo, usado).

Respuesta:

Si la actualización es exitosa, se devuelve la publicación actualizada.
Si no se puede encontrar la publicación o el usuario no tiene permisos, se devuelve un mensaje de error.
Marketplace (Ver todas las publicaciones)

GET /api/marketplace/publicaciones

Este endpoint permite a cualquier usuario autenticado ver todas las publicaciones disponibles en el marketplace.

Parámetros esperados:

Autorización: El token generado en el login debe enviarse en el encabezado de la solicitud.

Respuesta:

Devuelve una lista de todas las publicaciones disponibles en el marketplace.
Filtrar Marketplace por Categoría

GET /api/marketplace/publicaciones/{categoria}

Este endpoint permite ver todas las publicaciones dentro de una categoría específica.

Parámetros esperados:

Autorización: El token generado en el login debe enviarse en el encabezado de la solicitud.

Respuesta:

Devuelve una lista de publicaciones dentro de la categoría indicada.
Filtrar Marketplace por Precio Menor a un valor Específico

GET /api/marketplace/publicaciones/lt-{price}

Este endpoint permite ver todas las publicaciones cuyo precio sea menor al valor proporcionado.

Parámetros esperados:

Autorización: El token generado en el login debe enviarse en el encabezado de la solicitud.
price: Precio máximo para filtrar las publicaciones.

Respuesta:

Devuelve una lista de publicaciones con precios menores al valor indicado.
Filtrar Marketplace por Vendedor

GET /api/marketplace/publicaciones/sellerId-{sellerId}

Este endpoint permite ver todas las publicaciones de un vendedor específico.

Parámetros esperados:

Autorización: El token generado en el login debe enviarse en el encabezado de la solicitud.
sellerId: ID del vendedor.

Respuesta:

Devuelve una lista de publicaciones creadas por el vendedor indicado.
4. Controlador de Categorías
Crear Categorías (Solo Admin)

POST /api/categorias/crearCategoria

Este endpoint permite a un usuario administrador crear nuevas categorías para las publicaciones.

Solicitud:

{
    "name": "Equipo de Gimnasio",
    "slug": "equipo-gimnasio",
    "description": "Equipo para convertir tu casa en un mini gimnasio",
    "attributesDefinition": [
        {
            "key": "brand",
            "label": "Marca",
            "type": "string",
            "required": false
        },
        {
            "key": "type",
            "label": "Tipo de Equipo",
            "type": "select",
            "required": true,
            "options": ["Maquinas", "Mancuernas", "Poleas"]
        },
        {
            "key": "condition",
            "label": "Condición",
            "type": "select",
            "required": true,
            "options": ["Usado", "Buen Estado", "Excelente Estado"]
        }
    ]
}

Parámetros esperados:

name: Nombre de la categoría.
slug: Slug o identificador único para la categoría.
description: Descripción de la categoría.
attributesDefinition: Definición de atributos para productos dentro de esta categoría (por ejemplo, marca, tipo de equipo, condición, etc.).

Respuesta:

Si la categoría se crea correctamente, se devuelve un mensaje de éxito.
Si el usuario no es un administrador, se devuelve un error.
Obtener Todas las Categorías

GET /api/categorias/allCategorias

Este endpoint devuelve todas las categorías disponibles.

Parámetros esperados:

Autorización: El token del administrador debe enviarse en el encabezado de la solicitud.

Respuesta:

Devuelve una lista de todas las categorías existentes.
Conclusión

Con estos endpoints, la API permite gestionar el registro de usuarios, autenticación, creación y administración de publicaciones en el marketplace, así como la creación y gestión de categorías para las publicaciones. También permite realizar búsquedas avanzadas dentro del marketplace.

