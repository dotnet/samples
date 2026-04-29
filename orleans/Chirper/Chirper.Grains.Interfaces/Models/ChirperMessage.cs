namespace Chirper.Grains.Models;

/// <summary>
/// Data object representing one Chirp message entry
/// </summary>
[GenerateSerializer]
public record class ChirperMessage(
    /// <summary>
    /// The message content for this chirp message entry.
    /// </summary>
    [property: Id(0)] string Message,

    /// <summary>
    /// The timestamp of when this chirp message entry was originally republished.
    /// </summary>
    [property: Id(1)] DateTimeOffset Timestamp,

    /// <summary>
    /// The user name of the publisher of this chirp message.
    /// </summary>
    [property: Id(2)] string PublisherUserName)
{
    /// <summary>
    /// The unique id of this chirp message.
    /// </summary>
    [Id(3)]
    public Guid MessageId { get; } = Guid.NewGuid();

    /// <summary>
    /// Returns a string representation of this message.
    /// </summary>
    public override string ToString() =>
        $"Chirp: '{Message}' from @{PublisherUserName} at {Timestamp}";
}
