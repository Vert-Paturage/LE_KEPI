# Middleware

## Description
Le middleware sert d'intermédiaire pour l'appel de différents services au sein de l'ERP. Quand une application A a 
besoin de certaines données, elle fait appel au middleware via une demande précise, qui va appeler l'application B afin 
de retourner les données demandées.

Dans le fonctionnement du middleware, chaque application fournit une liste de codes d'action qu'elle peut exécuter, et 
ces codes sont utilisés par les autres applications pour effectuer ces actions.

### Exemple
Une application A permet de gérer les utilisateurs et peut réaliser deux actions :
- créer un utilisateur 
- retourner tous les utilisateurs

Elle fournit donc deux codes d'action :
- APP1_CREATE_USER
- APP1_GET_ALL_USERS.

Une application B qui souhaite afficher tous les utilisateurs fait appel au middleware en lui demandant d'exécuter 
l'action `APP1_GET_ALL_USERS`. Celui-ci appelle alors l'application A, qui lui retourne tous les utilisateurs. Le 
middleware renvoie ensuite ces utilisateurs à l'application B, sans que celle-ci ait besoin d'être au courant de 
l'existence de l'application A.


## Fonctionnement

Chaque application qui souhaite fonctionner avec le middleware doit respecter certaines spécifications.

Le middleware propose 3 endpoints :

- `register` : permet à une application de s'enregistrer auprès du middleware.
```js
{
  "appKey": "APP1", // identifiant unique de chaque application
  "url": "https://ip_application:port" // URL de l'application
}
```

- `action` : permet à une application de demander au middleware d'exécuter une action.
```js
{
  "key": "APP1_GET_ALL_USERS", // code de l'action
  "params": {
    "date": "2021-01-01"
  } | null, // paramètres d'URL et query params
  "body": {} | null // corps de la requête si besoin
}
```

- `my_swag` : permet de récupérer la documentation des actions enregistrées pour chaque application.


### L'application souhaite mettre à disposition des actions
Pour cela, elle doit fournir un endpoint `meuch_map` à la racine (exemple : https://ip_application:port/meuch_map), qui 
retourne la liste des actions qu'elle propose.

Chaque action dans la réponse de l'endpoint doit suivre la structure suivante :
```js
{
  "key": "APP1_GET_TEST", // code de l'action
  "endpoint" : "/test_endpoint", // endpoint de l'action depuis la racine
  "description" : "Endpoint de test", // petite description qui sera utilisée pour la documentation
  "type" : "GET", // type de la requête HTTP
  "routeFormat" : "/id/type" | null, // paramètres dans l'URL
  "queryParams" : ["date"] | null, // query params
  "body" : "{}" | null // structure du body
}
```

Exemple d'implémentation de l'endpoint en C# avec ASP.NET Core
```csharp
[HttpGet("test_endpoint/{id}/{type}")]
public IActionResult TestEndpoint(int id, string type, string date)
{
    return Ok($"Hello from the other side ({id}/{type} date={date})");
}
```

Exemple global avec l'endpoint `meuch_map` et d'autres endpoints
```csharp
[HttpGet("meuch_map")]
public IActionResult GetMeuch()
{
    List<MeuchEndpointInput> endpoints = new List<MeuchEndpointInput>()
    {
        new MeuchEndpointInput()
        {
            Key = "APP1_GET_TEST",
            Endpoint = "/test_endpoint",
            Description = "Endpoint de test",
            Type = "GET",
            RouteFormat = "/id/type",
            QueryParams = ["date"]
        },
        new MeuchEndpointInput()
        {
            Key = "APP1_CREATE",
            Endpoint = "/create_user",
            Description = "Créer un utilisateur",
            Type = "POST"
        }
    };
    return Ok(endpoints);
}

[HttpGet("test_endpoint/{id}/{type}")]
public IActionResult TestEndpoint(int id, string type, string date)
{
    return Ok($"Hello from the other side ({id}/{type} date={date})");
}

[HttpPost("create_user")]
public IActionResult CreateUser(CreateUserInput input)
{
    return Ok($"User created with name {input.Name}");
}
```

### Cinématique d'appel

Au démarrage d'une application, celle-ci doit s'enregistrer auprès du middleware pour signaler qu'elle est disponible.
Le middleware met alors à jour les actions qu'il connaît pour cette application avec la dernière version fournie lors 
de l'appel de l'endpoint `meuch_map`.

![cinematic.svg](img/cinematic.svg)

## TestApp

Le projet TestApp est une application de test qui permet de vérifier le fonctionnement du middleware. Elle simule le 
comportement d'une application utilisant le middleware.

[TestApp](src/Middleware.TestApp)


