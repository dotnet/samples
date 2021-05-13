# Status checker GitHub Action

This action checks for Open Publishing Build system build warnings.:rocket:

Whether or not there are build warnings, you'll see a status check for this action in your pull request. If it passes, it means it ran without error and there are not build warnings or errors. If it fails, it means there are build warnings or errors, or there's a bug in this action.

## Usage

Add a new YAML file to your repo in the *.github/workflows* folder. The only required input is the GitHub OAuth token.

For example, the following YAML example calls this status-checker Action.

```yml
on: [pull_request_target]

jobs:
  status_checker_job:
    runs-on: ubuntu-latest
    name: Checks the OpenPublishing.Build status check for build warnings
    steps:
    - uses: dotnet/samples/.github/actions/status-checker@main
      with:
        repo-token: ${{ secrets.GITHUB_TOKEN }}
```

Note: Use the `pull_request_target` event instead of the `pull_request` event as the trigger. The `pull_request_target` event runs against the workflow and code from the *base* of the pull request instead of from the merge commit. For more information, see [Improvements for public repository forks](https://github.blog/2020-08-03-github-actions-improvements-for-fork-and-pull-request-workflows/#improvements-for-public-repository-forks).
