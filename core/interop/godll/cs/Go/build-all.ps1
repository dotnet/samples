#------------------------------------------------------------------------------
# FILE:         build-all.ps1
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
# This script builds all GOLANG projects.
#
# USAGE: powershell -file build-all.ps1 [SOLUTION_DIR] [CONFIGURATION]
#
# ARGUMENTS:
#
#       -solutionDir        - The directory that the solution is in.  This
#                             is needed to build the Golang libraries.
#
#       -outDir             - The directory where build logs and archives
#                             will land.
#
#       -buildConfig Debug  - Optionally specifies the build configuration,
#                             either "Debug" or "Release".  This defaults
#                             to "Debug".

param 
(
    [Parameter(Position=0, Mandatory=$true)]
    [string]$solutionDir,
    [Parameter(Position=1, Mandatory=$false)]
    [string]$outDir = "bin\Debug\netcoreapp2.0\",
    [Parameter(Position=2, Mandatory=$false)]
    [string]$buildConfig = "Debug"
)

$orgDirectory = Get-Location

$env:BUILD_GOROOT = "$solutionDir\Go"
$env:BUILD_PATH   = "$solutionDir\SharedC\bin\$buildConfig\netcoreapp3.1"
$env:LOG_PATH     = "$solutionDir"+"Go\GoBuilder\$outDir"

echo $env:LOG_PATH

Set-Location $env:BUILD_GOROOT

./build-helloworld.ps1 -goRoot $env:BUILD_GOROOT -buildPath $env:BUILD_PATH -logPath $env:LOG_PATH -buildConfig $buildConfig
./build-math.ps1 -goRoot $env:BUILD_GOROOT -buildPath $env:BUILD_PATH -logPath $env:LOG_PATH -buildConfig $buildConfig

Set-Location $orgDirectory
