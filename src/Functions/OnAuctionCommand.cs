using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Wallymathieu.Auctions.ApiModels;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.Infrastructure.Queues;
namespace Wallymathieu.Auctions.Functions;
public class OnAuctionCommandHandler
{
    private readonly ICreateAuctionCommandHandler _createAuctionCommandHandler;
    private readonly ILogger _logger;
    private readonly JsonSerializerOptions _serializerOptions;

    public OnAuctionCommandHandler(ILoggerFactory loggerFactory,
        ICreateAuctionCommandHandler createAuctionCommandHandler,
        JsonSerializerOptions serializerOptions)
    {
        _createAuctionCommandHandler = createAuctionCommandHandler;
        _serializerOptions = serializerOptions;
        _logger = loggerFactory.CreateLogger<OnAuctionCommandHandler>();
    }

    [Function("OnAuctionCommand")]
    public async Task Run(
        [QueueTrigger(QueuesModule.AuctionCommandQueueName, Connection = "AzureWebJobsStorage")]
        string commandString, CancellationToken cancellationToken)
    {
        var command = JsonSerializer.Deserialize<CreateAuctionCommand>(commandString, _serializerOptions);
        _logger.LogInformation("Create auction command received");
        if (command == null) throw new NullReferenceException(nameof(command));
        var result = await _createAuctionCommandHandler.Handle(command, cancellationToken);
        _logger.LogInformation("Create auction command processed {AuctionId}", result.Id);
    }
}