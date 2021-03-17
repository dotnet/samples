FROM mcr.microsoft.com/dotnet/sdk:5.0

# Copy everything and restore
COPY . ./
RUN dotnet publish ./DotNet.GitHubAction/DotNet.GitHubAction.csproj -c Release -o out --no-self-contained

# Label the container
LABEL maintainer="David Pine <david.pine@microsoft.com>"
LABEL repository="https://github.com/dotnet/samples"
LABEL homepage="https://github.com/dotnet/samples"

LABEL com.github.actions.name=".NET code metric analyzer"
LABEL com.github.actions.description="A Github action that maintains a CODE_METRICS.md file, reporting cylcomatic complexity, maintainability index, etc."
LABEL com.github.actions.icon="sliders"
LABEL com.github.actions.color="purple"

COPY /out .
ENTRYPOINT [ "dotnet", "/DotNet.GitHubAction.dll" ]
