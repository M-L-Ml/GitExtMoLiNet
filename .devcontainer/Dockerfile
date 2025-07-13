FROM mcr.microsoft.com/dotnet/sdk:9.0

# Install necessary dependencies for GitExtensions
RUN apt-get update \
    && apt-get install -y \
        git \
        libx11-6 \
        # this is optional , needed for the ported Mono System.Windows.Forms for theme colors
        # but it includes libxinerama1 which is important
        libgtk2.0-0 \
        libgtk-3-0 \
        libgdiplus \
        libc6-dev \
        curl \
        wget \
        unzip \
        # those are just for convinience , in particular for debugging
        mc \
        nano \
        lsof \
        strace \
        htop
  
 # uncomment for freeing up some space
 #  rm -rf /var/lib/apt/lists/*

# RUN LAZYGIT_VERSION=$(curl -s "https://api.github.com/repos/jesseduffield/lazygit/releases/latest" | \grep -Po '"tag_name": *"v\K[^"]*') \
#     && curl -Lo lazygit.tar.gz "https://github.com/jesseduffield/lazygit/releases/download/v${LAZYGIT_VERSION}/lazygit_${LAZYGIT_VERSION}_Linux_x86_64.tar.gz" \
#     && tar xf lazygit.tar.gz lazygit \
#     && sudo install lazygit -D -t /usr/local/bin/ 

# Install Visual Studio Debugger (VSDBG) for remote debugging
RUN curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg

# Set up working directory
WORKDIR /workspace

# Verify .NET SDK installation
RUN dotnet --info
