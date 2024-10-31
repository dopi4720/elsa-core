docker build -t elsa-server:local -f ./docker/Dockerfile-elsa-server .
docker run -t -i -e ASPNETCORE_ENVIRONMENT='Development' -p 11000:8080 elsa-server:local