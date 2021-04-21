import { debug } from '@actions/core';
import * as github from '@actions/github';
import { wait } from './wait';

export async function checkStatus(token: string) {

  const octokit = github.getOctokit(token);
  const owner = github.context.repo.owner;
  const repo = github.context.repo.repo;
  const payload = github.context.payload;

  if (['pull_request', 'pull_request_target'].includes(github.context.eventName) && payload?.action) {
    const prNumber = payload.number;
    console.log({ prNumber });

    let sha;
    if (payload.action === 'synchronize') {
      sha = payload.after;
    }
    else if (['opened', 'reopened'].includes(payload.action)) {
      sha = payload.pull_request?.head.sha
    }
    else {
      console.log('Unexpected payload action.');
      return;
    }

    console.log({ sha });

    let buildStatus: any;

    // Get the completed build status.
    // Timeout after 10 minutes.
    for (let i = 0; i < 600; i += 10) {

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

      if (buildStatus != null) {
        if (buildStatus.state == 'pending') {
          console.log("Found OPS status check but it's still pending.")
          // Sleep for 10 seconds.
          await wait(10000);
          continue;
        }
        else {
          // Status is no longer pending.
          console.log("OPS status check has completed.")
          break;
        }
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
        return;
      }
    }
    else {
      if (buildStatus == null)
        console.log("Could not find the OpenPublishing.Build status check.");
      else {
        // Build status is error/failure, so merging will be blocked anyway.
        // We don't need to add another status check.
        console.log("OpenPublishing.Build status is either failure or error.");
      }

      // Don't create a new status check.
      return;
    }
  } else {
    console.log("Event is not a pull request or payload action is undefined.");
    return;
  }
}
