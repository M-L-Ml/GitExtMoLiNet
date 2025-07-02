FROM mcr.microsoft.com/dotnet/sdk:9.0

# Install necessary dependencies for GitExtensions
RUN apt-get update \
    && apt-get install -y \
        git \
        libx11-6 \
        libgtk-3-0 \
        libgdiplus \
        libc6-dev \
        curl \
        wget \
        unzip \
    && rm -rf /var/lib/apt/lists/*

# Set up working directory
WORKDIR /workspace

# Verify .NET SDK installation
RUN dotnet --info
