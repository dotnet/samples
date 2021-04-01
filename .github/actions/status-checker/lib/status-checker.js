"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    Object.defineProperty(o, k2, { enumerable: true, get: function() { return m[k]; } });
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __setModuleDefault = (this && this.__setModuleDefault) || (Object.create ? (function(o, v) {
    Object.defineProperty(o, "default", { enumerable: true, value: v });
}) : function(o, v) {
    o["default"] = v;
});
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (k !== "default" && Object.prototype.hasOwnProperty.call(mod, k)) __createBinding(result, mod, k);
    __setModuleDefault(result, mod);
    return result;
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.checkStatus = void 0;
const core_1 = require("@actions/core");
const github = __importStar(require("@actions/github"));
const wait_1 = require("./wait");
function checkStatus(token) {
    return __awaiter(this, void 0, void 0, function* () {
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
            let buildStatus;
            // Get the completed build status.
            for (let i = 0; i < 360; i += 10) {
                const { data: statuses } = yield octokit.repos.listCommitStatusesForRef({
                    owner: owner,
                    repo: repo,
                    ref: sha
                });
                // Get the most recent status.
                for (let status of statuses) {
                    if (status.context == 'OpenPublishing.Build') {
                        buildStatus = status;
                        core_1.debug("Found OPS status check.");
                        break;
                    }
                }
                if (buildStatus != null && buildStatus.state == 'pending') {
                    // Sleep for 10 seconds.
                    yield wait_1.wait(10000);
                    core_1.debug("State is still pending.");
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
                    return yield octokit.repos.createCommitStatus({
                        owner: owner,
                        repo: repo,
                        sha: sha,
                        state: 'failure',
                        context: 'Check for build warnings',
                        description: 'Please fix build warnings before merging.',
                    });
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
        }
        else {
            console.log("Unable to get GITHUB_SHA from the environment.");
            return null;
        }
    });
}
exports.checkStatus = checkStatus;
