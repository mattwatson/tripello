# tripello
Travel budget app to learn a bit of web development

# Command line cheat-sheet (linux)
dotnet dev-certs https -ep ${HOME}/.aspnet/https/Tripello.Server.Web.pfx -p <secretpassword>

dotnet build src/Tripello.Server.Web
dotnet run --project src/Tripello.Server.Web/

docker build -t tripello-server-web .

# Development configuration inclucing http for docker
docker run --rm -it -p 4000:80 -p 4001:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=4001 -e ASPNETCORE_ENVIRONMENT=Development -e ASPNETCORE_Kestrel__Certificates__Development__Password="<secretpassword>" -v ${HOME}/.aspnet/https:/root/.aspnet/https/ tripello-server-web

# Production configuration including https for docker
docker run --rm -it -p 8000:80 -p 8001:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=8001 -e ASPNETCORE_Kestrel__Certificates__Default__Password="<secretpassword>" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/Tripello.Server.Web.pfx -v ${HOME}/.aspnet/https:/https/ tripello-server-web