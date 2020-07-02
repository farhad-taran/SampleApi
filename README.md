


## How To Run
- run `docker-compose up --build` from root directory(this will bring up MySql and the Api)
- navigate to [http://localhost:5470/swagger/index.html](http://localhost:5470/swagger/index.html)
- running the api from visual studio will require the above first step and then you can navigate to : [http://localhost:5123/swagger/index.html](http://localhost:5123/swagger/index.html)

- acceptance tests are dependent on the docker containers to be running

- ping and healthcheck endpoints are tested in acceptance tests and can be hit at the following uris: /api/ping, /api/healthcheck

- serilog has been setup to add different log sinks, in dev and running locally it logs to a local file and colored console, if wanting to extend in prod, you can add or override in appropriate appsettings file

- flow charts are in the documents folder