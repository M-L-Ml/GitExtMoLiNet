FROM mcr.microsoft.com/dotnet/runtime:9.0

RUN apt-get update \
    && apt-get install -y libx11-6 \
    && rm -rf /var/lib/apt/lists/*
