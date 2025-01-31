# GROUPE I - Intégration

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
    "body" : "{}" | null, // structure du body
    "response" : "{}" | null // structure de la réponse
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
            QueryParams = ["date"],
            Response = "string"
        },
        new MeuchEndpointInput()
        {
            Key = "APP1_CREATE",
            Endpoint = "/create_user",
            Description = "Créer un utilisateur",
            Type = "POST",
            Response = JsonConvert.SerializeObject(new CreateUserResponse(), Formatting.Indented),
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
    return Ok(new CreateUserResponse()
    {
        Id = 1,
        Name = input.Name
    });    
}

internal sealed class CreateUserResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
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

## Exécuter le projet via Docker

Chaque projet (Middleware et TestApp) possède son Dockerfile pour être exécuté dans un conteneur Docker.

2 fichiers Docker compose sont présent dans le repository pour exécuter le projet :
- docker-compose.yml : pour exécuter uniquement le middleware
- docker-compose-all.yml : pour exécuter le middleware et l'app de test

#### Exécuter uniquement le middleware
```bash
docker compose build
docker compose up -d
```

#### Exécuter le middleware + test app
```bash
docker compose --file docker-compose-all.yml build
docker compose --file docker-compose-all.yml up -d
```

Ports :
- Middleware : 5000
- TestApp : 5001



# Conclusion
 Nous, le groupe I, avons implémenté le middleware pour l'intégration au sein de l'ERP. Ce middleware joue le rôle d'intermédiaire entre différentes applications, permettant à une application de récupérer des données d'une autre application sans avoir à connaître sa structure interne.

 Ce middleware n'est pas juste une solution technique, c’est un peu notre manifeste pour un monde plus fluide, plus interconnecté. On l’a conçu pour être l’ossature invisible mais indispensable des modules de l'ERP. En gros, c’est un peu comme un chef d’orchestre, qui fait en sorte que chaque instrument – ou chaque application – joue sa partition sans que l’une vienne perturber l’autre. Et croyez-moi, quand on parle de modules ERP, ça devient vite un ballet complexe de données qui dansent entre systèmes.

 On a fait en sorte que ce middleware soit à la fois souple, modulable et surtout évolutif, car le monde des applications, c'est comme un jeu de construction : on ajoute, on retire, on réajuste, et tout ça doit continuer à fonctionner sans accroc. On n’a pas juste codé une solution, on a codé la confiance entre des briques technologiques qui n’ont rien en commun, si ce n’est qu’elles veulent coopérer.

 Donc, oui, ce middleware, c’est notre bébé, et il a une belle vie devant lui. Il est là pour donner de la liberté à chaque module ERP tout en garantissant la stabilité et la fluidité des échanges. Une solution pas seulement technique, mais une nouvelle manière de voir la collaboration entre systèmes. On a posé les fondations, et maintenant, c’est à nous de voir jusqu’où ça peut aller. Et franchement, on est excités de voir l’avenir s’ouvrir à nous.


# Membres du projet
- Romain REYDEL 👑 (CHEF DE PROJET / DEVELOPPEUR / ARCHITECTE / TESTEUR / INTEGRATEUR / SUPPORT / DELICIEUX)
- Derya AY 💫
- Paul CHOPINET 😔
- Florian SPINDLER 😽
- Arthur VILLARD 🏐
