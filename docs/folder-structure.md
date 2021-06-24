# Folder structure

In this section the purpose of each root folder will be
explained. For each part there will be a more detailed
documentation. So this is just a basic overview.

## Constants

This folder contains constants that can be used through
the whole application. It prevents us from writing the
same variable values over and over again.

Example: This class contains error codes, that could
be written down several times in the code. Instead
we create a public constant and use it through the
whole application.

```csharp
namespace rest_api_blueprint.Constants.Identity
{
    public static class ErrorCodes
    {
        public const string FILETYPE_NOT_ALLOWED = "FILETYPE_NOT_ALLOWED";
        public const string ADDRESS_BELONGS_TO_ANOTHER_USER = "ADDRESS_BELONGS_TO_ANOTHER_USER";
        public const string ADDRESS_NOT_FOUND = "ADDRESS_NOT_FOUND";
    }
}
```

## Controllers

All endpoints (available uris) are defined in controllers.
Most of the authentication and authorization is handled in
controllers.

Example: PulicUserController

```csharp

namespace rest_api_blueprint.Controllers.Social
{
    /**
    * The whole controller matches to the path:
    * /social/publicuser
    */
    [Route("social/[controller]")]
    public class PublicUserController : DefaultControllerTemplate
    {
        ...
        /**
        * The route is an empty string here,
        * so it matches to the controllers path.
        */
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<PagedList<PublicUser>>> GetMultiple(
            [FromQuery] SearchParameters parameters
        )
        {
            ...
        }

        /**
        * Here we have a static path that will be appended to
        * the controllers base path like this:
        * /social/publicuser/admins
        */
        [HttpGet]
        [Route("admins")]
        public async Task<ActionResult<PublicUser>> GetAdmins()
        {
            ...
        }

        /**
        * This one has a dynamic part and contains a variable
        * that is forwarded to the function as a parameter
        * variable. The path is also appended to the controllers
        *  base path like this:
        * /social/publicuser/503ca73e-c1b1-4558-832b-ad122c921e81
        */
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<PublicUser>> GetOne(Guid id)
        {
            ...
        }
    }
}
```

## DbContexts

Here all database contexts are stored. It can be understood as
an external data source like a database for example. We use it
to define which entities are stored in which database table.

Additionally we can make some glocal configurations there.

## Entities

Entitities are a representation of our data model. It contains
each database table as an object including the relations to
each other.

## Exceptions

Every custom exception we want to throw in our application to
get a better tracabillity of potential errors are stored here.

## Filters

This is a very special one. Our controllers automatically
use the content type application/json. So it takes json and
returns json in each endpoint.

We want to be able to use the JsonPatchDocument which allows
us to make partial updates on an object. This is part of the
NewtonsoftJson package. We also want to use the default json
serializer which is way faster the the one from Newtonsoft
so we have to configure that the one from Newtonsoft is only
used for JsonPatchDocuments.

## Helpers

This folder contains extension methods that extend default
functionality. We will go through each one seperately.

## Models

This one contains that don't represent database tables. They
can be used to define endpoint parameters, return types,
environmental configuration, etc. We especially use it for
the automapper to map our entities to view models. Why we
are doing this and how it works is described in the
controller section.

## Profiles

We use the AutoMapper extension to automatically map from
one object to another. To speed this up it is nessecary to
define a profile so the mappings can be generated on build
time.

## Repositories

This is the main connection to the database. Each kind of
request to fetch data from the database is done here. We
also update our entities here.

## Services

This folder contains services that are dedicated to the
database, but can be connections to external services or
just centralized functionality.

## Static Services

Static services are only centralized functionality. These
services don't need any dependency injection and can run
statically.
