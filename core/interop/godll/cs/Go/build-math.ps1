#------------------------------------------------------------------------------
# FILE:         build-math.ps1
# CONTRIBUTOR:  John C Burns
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
#
# This script builds the [hello-world] GOLANG executables and writes
# them to specified build directory.
#
# USAGE: powershell -file build-helloworld.ps1 [GOROOT] [BUILD_PATH] [LOG_PATH] [CONFIGURATION]
#
# ARGUMENTS:
#
#       -goRoot             - The root directory of the Golang libraries
#                             to be build.
#
#       -buildPath          - The directory to put the built Golang
#                             executables.
#
#       -logPath            - Specifies the location where build log
#                             are kept.
#
#       -buildConfig Debug  - Optionally specifies the build configuration,
#                             either "Debug" or "Release".  This defaults
#                             to "Debug".

param 
(
    [Parameter(Position=0, Mandatory=$true)]
    [string]$goRoot,
    [Parameter(Position=1, Mandatory=$true)]
    [string]$buildPath,
    [Parameter(Position=2, Mandatory=$true)]
    [string]$logPath,
    [Parameter(Position=3, Mandatory=$false)]
    [string]$buildConfig = "Debug"
)

$env:GOPATH   =  $goRoot
$projectPath  = "$env:GOPATH\src\shared-c\cmd\math"
$orgDirectory = Get-Location

Set-Location $projectpath

if (!(test-path $buildPath))
{
    New-Item -ItemType Directory -Force -Path $buildPath
}

if (!(test-path $logPath))
{
    New-Item -ItemType Directory -Force -Path $logPath
}

# set log path to file
$logPath = "$logPath\build-math.log"

# Change to project path
Set-Location $projectPath

# Build the WINDOWS binary
$env:GOOS        = "windows"
$env:GOARCH      = "amd64"
$env:CGO_ENABLED = 1
go build -i -v -buildmode=c-shared -o $buildPath\math.win.dll "$projectPath\main.go" > "$logPath" 2>&1

$exitCode = $lastExitCode

if ($exitCode -ne 0)
{
    Write-Error "*** ERROR: [math] WINDOWS build failed.  Check build logs: $logPath"
    Set-Location $orgDirectory
    exit $exitCode
}

#---------------------------------------------------------------------

# Return to the original directory
Set-Location $orgDirectory

