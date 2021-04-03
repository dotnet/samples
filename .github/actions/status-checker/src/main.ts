const core = require('@actions/core');
import { wait } from './wait';
import { checkStatus } from './status-checker';

async function run(): Promise<void> {
  try {
    const token: string = core.getInput('repo-token')
    
    // Wait 60 seconds before checking status check result.
    await wait(60000)
    console.log('Waited 60 seconds.');
    
    await checkStatus(token);
  } catch (error) {
    core.setFailed(error.message)
  }
}

run()
