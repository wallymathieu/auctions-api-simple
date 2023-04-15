using Auctions.Domain;

namespace Tests;

public class TestData
{
    public static readonly AuctionId AuctionId = new(1L);
    public static readonly string Title = "auction";
    public static DateTimeOffset StartsAt = new DateTime(2016, 1, 1);
    public static DateTimeOffset EndsAt = new DateTime(2016, 2, 1);
    public static readonly UserId Seller = new("x1");
    public static readonly UserId Buyer = new("x2");

    public static TimedAscendingAuction GetAuction()=>
        new TimedAscendingAuction
        {
            AuctionId = AuctionId.Id,
            Title = Title,
            StartsAt = StartsAt,
            Expiry = EndsAt,
            User = Seller,
            Currency = new Currency(CurrencyCode.SEK),
            Options = 
            {
                MinRaise = Sek(1),
                TimeFrame = TimeSpan.FromMinutes(1),
                ReservePrice = Sek(100),
            }
        };

    public static Amount Sek(long a) =>
        new(Value: a,
            Currency: new Currency(CurrencyCode.SEK));

    public static Bid BidOf100 =>
        new Bid(AuctionId: AuctionId,
            User: Buyer,
            Amount: Sek(100L),
            At: new DateTime(2016, 1, 2));

    public static readonly UserId Buyer1 = new("x2");
    public static readonly UserId Buyer2 = new("x3");

    public static Bid Bid1 => new Bid(AuctionId: AuctionId,
        User: Buyer1,
        Amount: Amount.Parse("SEK10"),
        At: StartsAt.AddHours(1.0));

    public static Bid Bid2 => new Bid(AuctionId: AuctionId,
        User: Buyer2,
        Amount: Amount.Parse("SEK12"),
        At: StartsAt.AddHours(2.0));

    public static T WithBids<T>(T state) where T:IState
    {
        Assert.True(state.TryAddBid(Bid1.At, Bid1, out var e1),e1.ToString());
        Assert.True(state.TryAddBid(Bid2.At, Bid2, out var e2),e2.ToString());
        return state;
    }
}