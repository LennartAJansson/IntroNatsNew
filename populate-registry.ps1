REM How to pull images and push them to local registry
docker pull synadia/jsm:latest
docker tag synadia/jsm:latest registry.local:5000/synadia/jsm:latest
docker push registry.local:5000/synadia/jsm:latest
