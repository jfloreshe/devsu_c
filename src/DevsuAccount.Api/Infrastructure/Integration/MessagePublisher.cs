using MassTransit;

namespace DevsuAccount.Api.Infrastructure.Integration;

public class MessagePublisher(IBus bus) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await bus.Publish(new CurrentTime
            {
                Value = $"The current time is {DateTime.Now}"
            });

            await Task.Delay(1000, cancellationToken);
        }
    }
}

public record CurrentTime
{
    public string Value { get; init; } = string.Empty;
}