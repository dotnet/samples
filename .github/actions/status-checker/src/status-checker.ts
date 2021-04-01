import { debug } from '@actions/core';
import * as github from '@actions/github';
import { wait } from './wait';

export async function checkStatus(token: string) {

  const octokit = github.getOctokit(token);
  const owner = github.context.repo.owner;
  const repo = github.context.repo.repo;

  //console.log('context.ref is: ${github.context.ref}');

  //const { data: pullCommits } = await octokit.repos.listCommits({
  //  owner: owner,
  //  repo: repo,
  //  pull_number: github.context.ref
  //});

  //const sha: string = pullCommits[0].sha;
  const sha = process.env['GITHUB_SHA'] || null;

  if (sha) {

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
          debug("Found OPS status check.")
          break;
        }
      }

      if (buildStatus != null && buildStatus.state == 'pending') {
        // Sleep for 10 seconds.
        await wait(10000);
        debug("State is still pending.");
        continue;
      }
      else {
        // Status is no longer pending.
        break;
      }
    }

    if (buildStatus != null && buildStatus.state == 'success') {
      if (buildStatus.description == 'Validation status: warnings') {
        // Build has warnings, so add a new commit status with state=failure.
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
    console.log("Unable to get GITHUB_SHA from the environment.");
    return null;
  }
}
