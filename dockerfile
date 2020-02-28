FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY /app /app
WORKDIR /app
EXPOSE 5000/tcp
ENTRYPOINT ["dotnet", "MVCWebApplication.dll", "--server.urls", "http://0.0.0.0:5000"]
