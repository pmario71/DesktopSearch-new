param
(
    [ValidateSet('all','tika', 'tika_local','elastic')]
    [string]$serviceToStart='tika'
)

$path = "D:\Tools\Allgemein\tika-server-1.16.jar"

switch ($serviceToStart)
{
    'all' 
    { 
        docker-compose.exe up
    }
    'tika'
    {
        docker run -d -p 9998:9998 docker-tikaserver_mp #logicalspark/docker-tikaserver
    }
	'tika_local'
    {
        $res = Invoke-WebRequest -Uri "http://localhost:9998"
        if ($res.StatusCode -ne 200)
        {
            java -jar $path -s
        }
        #docker run -d -p 9998:9998 docker-tikaserver_mp #logicalspark/docker-tikaserver
    }
    'elastic'
    {
        # -p 5044:5044  add to allow logstash to communicate
        docker run -d -v c:/Index:/var/lib/elasticsearch -e LOGSTASH_START=0 -p 5601:5601 -p 9200:9200 sebp/elk
    }
}