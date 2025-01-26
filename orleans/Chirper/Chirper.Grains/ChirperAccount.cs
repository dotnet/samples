﻿using System.Collections.Immutable;
using Chirper.Grains.Models;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Runtime;

namespace Chirper.Grains;

[Reentrant]
public sealed class ChirperAccount(
   [PersistentState(stateName: "account", storageName: "AccountState")] IPersistentState<ChirperAccountState> state,
   ILogger<ChirperAccount> logger) : Grain, IChirperAccount
{
    private static string GrainType => nameof(ChirperAccount);

    /// <summary>
    /// Size for the recently received message cache.
    /// </summary>
    private const int ReceivedMessagesCacheSize = 100;

    /// <summary>
    /// Size for the published message cache.
    /// </summary>
    private const int PublishedMessagesCacheSize = 100;

    /// <summary>
    /// Max length of each chirp.
    /// </summary>
    private const int MaxChirpLength = 280;

    /// <summary>
    /// Holds the transient list of viewers.
    /// This list is not part of state and will not survive grain deactivation.
    /// </summary>
    private readonly HashSet<IChirperViewer> _viewers = new();

    /// <summary>
    /// Allows state writing to happen in the background.
    /// </summary>
    private Task? _outstandingWriteStateOperation;

    private string GrainKey => this.GetPrimaryKeyString();

    public override Task OnActivateAsync(CancellationToken _)
    {
        logger.LogInformation("{GrainType} {GrainKey} activated.", GrainType, GrainKey);

        return Task.CompletedTask;
    }

    public async ValueTask PublishMessageAsync(string message)
    {
        var chirp = CreateNewChirpMessage(message);

        logger.LogInformation("{GrainType} {GrainKey} publishing new chirp message '{Chirp}'.",
            GrainType, GrainKey, chirp);

        state.State.MyPublishedMessages.Enqueue(chirp);

        while (state.State.MyPublishedMessages.Count > PublishedMessagesCacheSize)
        {
            state.State.MyPublishedMessages.Dequeue();
        }

        await WriteStateAsync();

        // notify viewers of new message
        logger.LogInformation("{GrainType} {GrainKey} sending new chirp message to {ViewerCount} viewers.",
            GrainType, GrainKey, _viewers.Count);

        _viewers.ForEach(_ => _.NewChirp(chirp));

        // notify followers of a new message
        logger.LogInformation("{GrainType} {GrainKey} sending new chirp message to {FollowerCount} followers.",
            GrainType, GrainKey, state.State.Followers.Count);

        await Task.WhenAll(state.State.Followers.Values.Select(_ => _.NewChirpAsync(chirp)).ToArray());
    }

    public ValueTask<ImmutableList<ChirperMessage>> GetReceivedMessagesAsync(int number, int start)
    {
        if (start < 0) start = 0;
        if (start + number > state.State.RecentReceivedMessages.Count)
        {
            number = state.State.RecentReceivedMessages.Count - start;
        }

        return ValueTask.FromResult(
            state.State.RecentReceivedMessages
                .Skip(start)
                .Take(number)
                .ToImmutableList());
    }

    public async ValueTask FollowUserIdAsync(string username)
    {
        logger.LogInformation(
            "{GrainType} {UserName} > FollowUserName({TargetUserName}).",
            GrainType,
            GrainKey,
            username);

        var userToFollow = GrainFactory.GetGrain<IChirperPublisher>(username);

        await userToFollow.AddFollowerAsync(GrainKey, this.AsReference<IChirperSubscriber>());

        state.State.Subscriptions[username] = userToFollow;

        await WriteStateAsync();

        // notify any viewers that a subscription has been added for this user
        _viewers.ForEach(cv => cv.SubscriptionAdded(username));
    }

    public async ValueTask UnfollowUserIdAsync(string username)
    {
        logger.LogInformation(
            "{GrainType} {GrainKey} > UnfollowUserName({TargetUserName}).",
            GrainType,
            GrainKey,
            username);

        // ask the publisher to remove this grain as a follower
        await GrainFactory.GetGrain<IChirperPublisher>(username)
            .RemoveFollowerAsync(GrainKey);

        // remove this publisher from the subscriptions list
        state.State.Subscriptions.Remove(username);

        // save now
        await WriteStateAsync();

        // notify event subscribers
        _viewers.ForEach(cv => cv.SubscriptionRemoved(username));
    }

    public ValueTask<ImmutableList<string>> GetFollowingListAsync() =>
        ValueTask.FromResult(state.State.Subscriptions.Keys.ToImmutableList());

    public ValueTask<ImmutableList<string>> GetFollowersListAsync() =>
        ValueTask.FromResult(state.State.Followers.Keys.ToImmutableList());

    public ValueTask SubscribeAsync(IChirperViewer viewer)
    {
        _viewers.Add(viewer);
        return ValueTask.CompletedTask;
    }

    public ValueTask UnsubscribeAsync(IChirperViewer viewer)
    {
        _viewers.Remove(viewer);
        return ValueTask.CompletedTask;
    }

    public ValueTask<ImmutableList<ChirperMessage>> GetPublishedMessagesAsync(int number, int start)
    {
        if (start < 0) start = 0;
        if (start + number > state.State.MyPublishedMessages.Count)
        {
            number = state.State.MyPublishedMessages.Count - start;
        }
        return ValueTask.FromResult(
            state.State.MyPublishedMessages
                .Skip(start)
                .Take(number)
                .ToImmutableList());
    }

    public async ValueTask AddFollowerAsync(string username, IChirperSubscriber follower)
    {
        state.State.Followers[username] = follower;
        await WriteStateAsync();
        _viewers.ForEach(cv => cv.NewFollower(username));
    }

    public ValueTask RemoveFollowerAsync(string username)
    {
        state.State.Followers.Remove(username);
        return WriteStateAsync();
    }

    public async Task NewChirpAsync(ChirperMessage chirp)
    {
        logger.LogInformation(
            "{GrainType} {GrainKey} received chirp message = {Chirp}",
            GrainType,
            GrainKey,
            chirp);

        state.State.RecentReceivedMessages.Enqueue(chirp);

        // only relevant when not using fixed queue
        while (state.State.RecentReceivedMessages.Count > ReceivedMessagesCacheSize) // to keep not more than the max number of messages
        {
            state.State.RecentReceivedMessages.Dequeue();
        }

        await WriteStateAsync();

        // notify any viewers that a new chirp has been received
        logger.LogInformation(
            "{GrainType} {GrainKey} sending received chirp message to {ViewerCount} viewers",
            GrainType,
            GrainKey,
            _viewers.Count);

        _viewers.ForEach(_ => _.NewChirp(chirp));
    }

    private ChirperMessage CreateNewChirpMessage(string message) =>
        new(message, DateTimeOffset.UtcNow, GrainKey);

    // When reentrant grain is doing WriteStateAsync, etag violations are possible due to concurrent writes.
    // The solution is to serialize and batch writes, and make sure only a single write is outstanding at any moment in time.
    private async ValueTask WriteStateAsync()
    {
        if (_outstandingWriteStateOperation is Task currentWriteStateOperation)
        {
            try
            {
                // await the outstanding write, but ignore it since it doesn't include our changes
                await currentWriteStateOperation;
            }
            catch
            {
                // Ignore all errors from this in-flight write operation, since the original caller(s) of it will observe it.
            }
            finally
            {
                if (_outstandingWriteStateOperation == currentWriteStateOperation)
                {
                    // only null out the outstanding operation if it's the same one as the one we awaited, otherwise
                    // another request might have already done so.
                    _outstandingWriteStateOperation = null;
                }
            }
        }

        if (_outstandingWriteStateOperation is null)
        {
            // If after the initial write is completed, no other request initiated a new write operation, do it now.
            currentWriteStateOperation = state.WriteStateAsync();
            _outstandingWriteStateOperation = currentWriteStateOperation;
        }
        else
        {
            // If there were many requests enqueued to persist state, there is no reason to enqueue a new write 
            // operation for each, since any write (after the initial one that we already awaited) will have cumulative
            // changes including the one requested by our caller. Just await the new outstanding write.
            currentWriteStateOperation = _outstandingWriteStateOperation;
        }

        try
        {
            await currentWriteStateOperation;
        }
        finally
        {
            if (_outstandingWriteStateOperation == currentWriteStateOperation)
            {
                // only null out the outstanding operation if it's the same one as the one we awaited, otherwise
                // another request might have already done so.
                _outstandingWriteStateOperation = null;
            }
        }
    }
}
