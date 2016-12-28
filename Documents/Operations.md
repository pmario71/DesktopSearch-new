# Operations

## Starting all necessary services

* use script which doing a `docker-compose up` internally

```bat
cd <Documents>
.\StartServices.ps1
```

## Test if services were started sucessfully

* ElasticSearch:   <http://localhost:9200/>
* Tika:   <http://http://localhost:9998/>
* Kibana:   <http://localhost:9200/>

## Index location

Index information is written into mounted volume:\
`c:\Index`

## Used Docker Images

| Name        | Image ID                       | Description                                             |
|-------------|--------------------------------|---------------------------------------------------------|
| ELK Stack   | sebp/elk                       | <https://hub.docker.com/r/sebp/elk><br><https://elk-docker.readthedocs.io>                |
| Tika Server | logicalspark/docker-tikaserver | <https://hub.docker.com/r/logicalspark/docker-tikaserver> |