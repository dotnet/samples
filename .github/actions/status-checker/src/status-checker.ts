import { debug } from '@actions/core';
import * as github from '@actions/github';
import { wait } from './wait';

export async function checkStatus(token: string) {

  const octokit = github.getOctokit(token);
  const owner = github.context.repo.owner;
  const repo = github.context.repo.repo;
  const prNumber = github.context.payload.pull_request?.number || null;
  console.log({ prNumber });

  if (prNumber) {

    const { data: pullCommits } = await octokit.rest.pulls.listCommits({
      owner: owner,
      repo: repo,
      pull_number: prNumber
    });

    const sha: string = pullCommits[0].sha;
    console.log({ sha });

    let buildStatus: any;

    // Get the completed build status.
    for (let i = 0; i < 360; i += 10) {

      const { data: statuses } = await octokit.repos.listCommitStatusesForRef({
        owner: owner,
        repo: repo,
        ref: sha
      });

      // Get the most recent status.
      for (let status of statuses) {
        if (status.context == 'OpenPublishing.Build') {
          buildStatus = status;
          break;
        }
      }

      if (buildStatus != null && buildStatus.state == 'pending') {
        console.log("Found OPS status check but it's still pending.")
        // Sleep for 10 seconds.
        await wait(10000);
        continue;
      }
      else {
        // Status is no longer pending.
        console.log("OPS status check is no longer in pending state.")
        break;
      }
    }

    if (buildStatus != null && buildStatus.state == 'success') {
      if (buildStatus.description == 'Validation status: warnings') {
        // Build has warnings, so add a new commit status with state=failure.
        console.log('Found build warnings.');

        return await octokit.repos.createCommitStatus({
          owner: owner,
          repo: repo,
          sha: sha,
          state: 'failure',
          context: 'Check for build warnings',
          description: 'Please fix build warnings before merging.',
        })
      }
      else {
        console.log("OpenPublishing.Build status check did not have warnings.");
        // Don't create a new status check.
        return null;
      }
    }
    else {

      if (buildStatus == null)
        console.log("Could not find the OpenPublishing.Build status check.");
      else {
        // Build status is error, so merging will be blocked anyway.
        // We don't need to add another status check.
        console.log("OpenPublishing.Build status is either failure or error.");
      }

      // Don't create a new status check.
      return null;
    }
  } else {
    console.log("Unable to get pull request number from context payload.");
    return null;
  }
}
