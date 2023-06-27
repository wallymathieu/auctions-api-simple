global using Xunit;
global using static Wallymathieu.Auctions.Tests.TestData;
global using ICreateBidCommandHandler= Wallymathieu.Auctions.Infrastructure.CommandHandlers.ICommandHandler<
    Wallymathieu.Auctions.Commands.CreateBidCommand,
    Wallymathieu.Auctions.Result<Wallymathieu.Auctions.DomainModels.Bid, Wallymathieu.Auctions.DomainModels.Errors>>;
global using ICreateAuctionCommandHandler= Wallymathieu.Auctions.Infrastructure.CommandHandlers.ICommandHandler<
    Wallymathieu.Auctions.Commands.CreateAuctionCommand,
    Wallymathieu.Auctions.DomainModels.Auction>;