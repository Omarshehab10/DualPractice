# Set base image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# install wget 
RUN apt-get update \
    && apt-get install -y wget \
    && rm -rf /var/lib/apt/lists/*

# make links to libdl.so and libgdiplus.so
RUN ln -s /lib/x86_64-linux-gnu/libdl.so.2 /lib/x86_64-linux-gnu/libdl.so
RUN ln -s /usr/lib/libgdiplus.so /lib/x86_64-linux-gnu/libgdiplus.so

# Set working directory
WORKDIR /app

# Copy manifest and dependencies
COPY . .

# Restore the dependencies and tools of a project
RUN dotnet restore 

# Publishes the application and its dependencies using Release configuration to /app/out 
RUN dotnet publish --configuration Release --output /app/out

# Set base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

# install wget 
RUN apt-get update && apt-get install -y wget && rm -rf /var/lib/apt/lists/*

ENV TZ Asia/Riyadh

# Set working directory
WORKDIR /app

# Install libwkhtmltox dependencies
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        zlib1g \
        fontconfig \
        libfreetype6 \
        libx11-6 \
        libxext6 \
        libxrender1 \
        libjpeg62-turbo \
        libgdiplus
ADD /DualPractitionerBE/libwkhtmltox.dll .
ADD /DualPractitionerBE/libwkhtmltox.so .
ADD /DualPractitionerBE/libwkhtmltox.dylib .

# Listen on port 80 at runtime 
EXPOSE 80

# Copy project
COPY --from=build-env /app/out .

# Set default startup command
CMD  ["dotnet", "DualPractitionerBE.dll"]
