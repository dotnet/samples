# Status checker GitHub Action

Use this action to check for Open Publishing Build system warnings.:rocket:

This action only adds a status check to a pull request if the "OpenPublishing.Build" status check contains warnings. The new status check will have a state of "failure" so that you can more easily identify build warnings at a glance.

## Usage

Add a new YAML file to your repo in the *.github/workflows* folder. The only required input is the GitHub OAuth token.

For example, the following YAML example calls this status-checker Action.

```yml
on: [pull_request]

jobs:
  status_checker_job:
    runs-on: ubuntu-latest
    name: Checks the OpenPublishing.Build status check for build warnings
    steps:
    - uses: actions/checkout@v1
    - uses: dotnet/samples/.github/actions/status-checker@main
      with:
        repo-token: ${{ secrets.GITHUB_TOKEN }}
```
