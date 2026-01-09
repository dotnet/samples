using GPSTracker.Common;
using GPSTracker.GrainInterface;
using Orleans.Concurrency;
using Orleans.Runtime;

namespace GPSTracker.GrainImplementation;

[Reentrant]
[StatelessWorker(maxLocalWorkers: 12)]
public sealed class PushNotifierGrain(ILogger<PushNotifierGrain> logger) : Grain, IPushNotifierGrain, IDisposable
{
    private readonly Queue<VelocityMessage> _messageQueue = new();
    private List<(SiloAddress Host, IRemoteLocationHub Hub)> _hubs = new();
    private Task _flushTask = Task.CompletedTask;
    private IGrainTimer? _flushTimer;
    private IGrainTimer? _refreshTimer;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        // Set up a timer to regularly flush the message queue
        _flushTimer = this.RegisterGrainTimer(
            ct => Flush(),
            dueTime: TimeSpan.FromMilliseconds(15),
            period: TimeSpan.FromMilliseconds(15));

        // Set up a timer to regularly refresh the hubs, to respond to azure infrastructure changes
        await RefreshHubs();

        _refreshTimer = this.RegisterGrainTimer(
            async _ => await RefreshHubs(),
            dueTime: TimeSpan.FromSeconds(60),
            period: TimeSpan.FromSeconds(60));

        await base.OnActivateAsync(cancellationToken);
    }


    public override async Task OnDeactivateAsync(DeactivationReason deactivationReason, CancellationToken cancellationToken)
    {
        await Flush();
        await base.OnDeactivateAsync(deactivationReason, cancellationToken);
    }

    public void Dispose()
    {
        _flushTimer?.Dispose();
        _refreshTimer?.Dispose();
    }

    private async ValueTask RefreshHubs()
    {
        // Discover the current infrastructure
        IHubListGrain hubListGrain = GrainFactory.GetGrain<IHubListGrain>(Guid.Empty);
        _hubs = await hubListGrain.GetHubs();
    }

    public ValueTask SendMessage(VelocityMessage message)
    {
        // Add a message to the send queue
        _messageQueue.Enqueue(message);
        return new(Flush());
    }

    private Task Flush()
    {
        if (_flushTask.IsCompleted)
        {
            _flushTask = FlushInternal();
        }

        return _flushTask;

        async Task FlushInternal()
        {
            const int MaxMessagesPerBatch = 100;
            if (_messageQueue.Count == 0) return;

            while (_messageQueue.Count > 0)
            {
                // Send all messages to all SignalR hubs
                var messagesToSend = new List<VelocityMessage>(Math.Min(_messageQueue.Count, MaxMessagesPerBatch));
                while (messagesToSend.Count < MaxMessagesPerBatch && _messageQueue.TryDequeue(out VelocityMessage? msg))
                {
                    messagesToSend.Add(msg);
                }

                var batch = new VelocityBatch(messagesToSend);
                var tasks = _hubs.Select(hub => BroadcastUpdates(hub.Host, hub.Hub, batch, logger));

                await Task.WhenAll(tasks);
            }
        }
    }

    private static async Task BroadcastUpdates(SiloAddress host, IRemoteLocationHub hub, VelocityBatch batch, ILogger logger)
    {
        try
        {
            await hub.BroadcastUpdates(batch);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error broadcasting to host {Host}", host);
        }
    }
}
