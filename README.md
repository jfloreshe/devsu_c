# Microservices Challenge
## Tecnologías Utilizadas
- .NET 8.0
- Docker
  - Docker Compose
- RabbitMQ
- SQL Server

## Descripción
Este repositorio contiene dos microservicios independientes: Clientes y Cuentas. La comunicación asíncrona entre ellos se realiza mediante RabbitMQ, permitiendo el intercambio de eventos del dominio del microservicio de Clientes (Productor). Al crear, actualizar o eliminar un cliente, se envía un evento al microservicio de Cuentas (Consumidor) para asegurar la consistencia de datos en los reportes de transacciones.

## Arquitectura
La arquitectura del proyecto es híbrida, diseñada para combinar principios de modularización, bajo acoplamiento y alta cohesión entre módulos y submódulos. Implementa conceptos de:

- DDD (Domain-Driven Design) para estructurar el dominio de forma clara y organizada.
- VSA (Vertical Slice Architecture) para agrupar el código relacionado por casos de uso, promoviendo una estructura modular.
- Mediator (un patrón de diseño que permite una comunicación de bajo acoplamiento entre objetos).
- Patrones de Diseño y Arquitectura Orientada a Eventos para la comunicación entre microservicios.

## Configuración
Este proyecto está preparado para ser desplegado a través de **Docker Compose**.

### Requisitos previos
- Docker y Docker Compose instalados.
- Tener SDK 8 de Net
  
### Pasos de configuración y despliegue
#### Despliegue Automático:

- Navega a la carpeta **./devops**.
- Ejecuta el siguiente comando en la terminal: ```docker compose up --build -d```
Esto levantará ambos microservicios, RabbitMQ y SQL Server en contenedores, y aplicará automáticamente las migraciones de la base de datos necesarias.

#### Despliegue Manual:

- Si deseas ejecutar cada proyecto individualmente, actualiza las variables de configuración en appsettings.json (para la conexión a RabbitMQ y SQL Server).

### Consideraciones
- Migraciones Automáticas: Las migraciones de la base de datos se aplican automáticamente en el despliegue.
- No se proporciona un seed de datos inicial. Si necesitas datos de prueba, puedes agregarlos manualmente una vez desplegado el entorno.
- La imágen de SqlServer puede demorar un poco en completar la descarga
- El proyecto puede que no levante con un delay, ya que esperamos a que levante RabbitMq y la base de datos para continuar con el despliegue de los microservicios
- En la carpeta **Entregables** encontrará 2 archivos .json que son la prueba de todos los endpoints con POSTMAN
