# Fluendo Position Challenge

[![N|Solid](https://web-fluendo.s3.amazonaws.com/static/img/header/logo-fluendo.png)](https://fluendo.com/en/)

# Challenge Explanation
## Components
### PUBG.Stats.API
This is the public ASP.NET Core 2.2 Web API that retrieves data from Mongo Instance and in some endpoints interact with the backgound worker.

The authorization is made using a custom filter that simply check a fixed and configurable Token. In a real environment that should be implemented using for example some JWT system (with other API issuing and signing tokens sticked to users).

There is a swagger endpoint that you can find at http://localhost:5000/swagger .

The configuration is done following appsettings Microsoft conventions and we inject the ENV variable that specifies which env is at docker-compose level.

Also the responses are cached using CacheAside against Redis.

### PUBG.Stats.Worker
This is the long running background task that each configurable minutes performs a refresh of the stats agains PUBG API.

There is long running task configured and hooked up to the application lifetime events (following how Microsoft Generic Host works), this is done manually for one reason, I want to keep a internal API to communicate with the process , so the PUBG.Stats.Worker is really an hybrid host, it has both a worker an a full functional internal WEB API (on docker compose we are not exposing any port outside).

Its API lets get adhoc data following reqs of one of the endpoints and forcing the refresh.

### Services and Core libs
Here are the common libraries using in more than one place, there are thing like repo acess and bootstrap helpers for the hosts.

### Tests

There are few tests, this has one reason. Since it is job challenge I assumed that you want to see my skills working with Mocking Frameworks, tests structures, etc... So for this reason I've created a minimum set of working tests, also I ran out of time during the weekend. In my day to day I have anm strong test philosophy trying to cover all my code with useful tests and integration tests, testing and reliable code are in my top priorities. If needed I can extend more in the interview.

### Mongo and Redis

Added images at docker compose level, Mongo has its own volume to persists data between docker-compose up's .
Each component has its own alias to be accessed.

### Key points
  - Tried to follow SOLID principles, good coding practises, custom exceptions, defensive programming in some points, async/await etc...
  - Used Moq and NUnit for unit testing.
  - Used ENV variable to specify which appsetting use, there are nothing hardcoded that depends on 3rd parties.
  - Swagger implemented.
  - Rate limit implemented following PUBG specs (1 req each 6 seconds).
  - Public api is using cache and performing well.
  - The code is at github.

### Out-of-scope
I would like to introduce one more thing that is some SDK library with the models in order to make easy API integrations but I leave the API responses as the MongoDocuments entities. In a real world application I would implemented a mapping system between our domain entitiy (in this Mongo Documents) and out API responses filtering out field that we don't want to expose to the public for any reason.


### Self-criticism and Self-evaluation 

I have tried to do the most things as I know during the weekend and trying to use the major part tech stack that I know, I think the test is hughe and with a lot of pieces and components so I am sure that on some points I can improve the implementations with a revisit (I like to revisit complex implementations) and maybe there are corner cases that are not cover as well as I can do it in a normal work environment but I wanted to get the challenge done and working stable inside the weekend.

Also it was my first time implementing a data layer with Mongo, I have worked with Elasticsearch as NoSql database but I thought it was a good challenge to implement that with Mongo.

Thanks for the opportunity, as I videogame lover and mad programmer I enjoyed a lot this challenge :) . 



### Installation

#### Via Visual Studio
If you want to open and debug solution using Visual Studio you need the following installed before :

  - Redis at default port.
  - Mongo at default port and :
     - pubg-stats database created.
     - last-completed-execution , leaderboard, lifetime-stats collections created **.
 - ASPNETCORE_ENVIRONMENT variable set to Development (inside csproj configuration , Debug tab)

Then you only need to set up PUBG.Stats.API and PUBG.Stats.Worker as startup projects.
The public API is at http://localhost:5000

#### Via docker compose
If you want to run docker compose you first need the following :
  - Docker installed with Linux containers.

Then you need to be on the solution root path and run
```sh
$ docker-compose build --no-cache 
$ docker-compose up
```
