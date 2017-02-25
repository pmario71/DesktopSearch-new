param
(
    [ValidateSet('all','tika','elastic')]
    [string]$serviceToStart='all'
)

switch ($serviceToStart)
{
    'all' 
    { 
        docker-compose.exe up
    }
    'tika'
    {
        docker run -d -p 9998:9998 logicalspark/docker-tikaserver
    }
    'elastic'
    {
        # -p 5044:5044  add to allow logstash to communicate
        docker run -d -v c:/Index:/var/lib/elasticsearch -e LOGSTASH_START=0 -p 5601:5601 -p 9200:9200 sebp/elk
    }
}