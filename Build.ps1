docker build -f .\NatsApi\Dockerfile --force-rm -t natsapi .
docker build -f .\NatsConsumer\Dockerfile --force-rm -t natsconsumer .

docker tag natsapi:latest $env:REGISTRYHOST/natsapi:latest
docker tag natsconsumer:latest $env:REGISTRYHOST/natsconsumer:latest

docker push $env:REGISTRYHOST/natsapi:latest
docker push $env:REGISTRYHOST/natsconsumer:latest
