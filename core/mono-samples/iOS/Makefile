MONO_CONFIG=Debug
MONO_ARCH=x64
USE_LLVM=True

all: run

TOOLS_DIR=../mobile.tasks
appbuilder:
	dotnet build -c Debug $(TOOLS_DIR)/AppleAppBuilder/AppleAppBuilder.csproj

runtimepack:
	../../../../.././build.sh Mono+Libs -os iOS -arch $(MONO_ARCH) -c $(MONO_CONFIG)

run: clean appbuilder
	dotnet publish -c $(MONO_CONFIG) /p:TargetArchitecture=$(MONO_ARCH) \
	/p:UseLLVM=$(USE_LLVM) /p:UseAotForSimulator=false

clean:
	rm -rf bin
