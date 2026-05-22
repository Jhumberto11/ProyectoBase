create keyspace marketplace 
WITH REPLICATION = 
{ 'class' : 'SimpleStrategy',
 'replication_factor' : 3
}


use marketplace;

CREATE TABLE IF NOT EXISTS mensajes_por_conversacion (
    conversacion_id text,
    fecha_envio timestamp,
    mensaje_id text,
    remitente_id text,
    destinatario_id text,
    publicacion_id text,
    contenido text,
    leido boolean,
    PRIMARY KEY ((conversacion_id), fecha_envio, mensaje_id)
) WITH CLUSTERING ORDER BY (fecha_envio ASC);



CREATE TABLE IF NOT EXISTS conversaciones_por_usuario (
    usuario_id text,
    ultima_fecha timestamp,
    conversacion_id text,
    otro_usuario_id text,
    publicacion_id text,
    ultimo_mensaje text,
    PRIMARY KEY ((usuario_id), ultima_fecha, conversacion_id)
) WITH CLUSTERING ORDER BY (ultima_fecha DESC);

CREATE TABLE IF NOT EXISTS historial_login (
    usuario_id text,
    fecha timestamp,
    estado boolean,     --si fallo o no    
    PRIMARY KEY ((usuario_id), fecha)
) WITH CLUSTERING ORDER BY (fecha DESC);




select * from conversaciones_por_usuario;
select * from mensajes_por_conversacion;
select * from historial_login;