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

    const commit = payload.pull_request?.head.sha;
    console.log({ commit });

    let buildStatus: any;
    
    const { data: statuses } = await octokit.repos.listCommitStatusesForRef({
      owner: owner,
      repo: repo,
      ref: commit
    });

    // Get the most recent OPS status.
    for (let status of statuses) {
      if (status.context == 'OpenPublishing.Build') {
        buildStatus = status;
        break;
      }
    }
    
    // Didn't find OPS status. This is bad.
    if (buildStatus == null)
    {
      throw new Error('Did not find OPS status check.')
    }
    
    // Check state of OPS status check.
    while (buildStatus.state == 'pending') {
      console.log("Found OPS status check in pending state.")
      
      // Sleep for 10 seconds.
      await wait(10000);
      
      // Get latest OPS status.
      const { data: statuses } = await octokit.repos.listCommitStatusesForRef({
        owner: owner,
        repo: repo,
        ref: commit
      });
      
      buildStatus = null;
      for (let status of statuses) {
        if (status.context == 'OpenPublishing.Build') {
          buildStatus = status;
          break;
        }
      }
      
      // This should never happen since if nothing else,
      // we'll find the OPS status we found initially.
      if (buildStatus == null)
      {
        throw new Error('Did not find OPS status check.')
      }
    }
    
    // Status is no longer pending.
    console.log("OPS status check has completed.")

    if (buildStatus.state == 'success') {
      if (buildStatus.description == 'Validation status: warnings') {
        // Build has warnings, so add a new commit status with state=failure.
        console.log('Found build warnings.');

        return await octokit.repos.createCommitStatus({
          owner: owner,
          repo: repo,
          sha: commit,
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
      // Build status is error/failure, so merging will be blocked anyway.
      // We don't need to add another status check.
      console.log("OpenPublishing.Build status is either failure or error.");

      // Don't create a new status check.
      return;
    }
  } else {
    console.log("Event is not a pull request or payload action is undefined.");
    return;
  }
}
