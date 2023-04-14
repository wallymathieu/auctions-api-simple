using System.Text.RegularExpressions;

namespace Auctions.Domain;

[Serializable]
public abstract record User(UserId Id)
{
    [Serializable]
    public record BuyerOrSeller(UserId Id, string? Name) : User(Id)
    {
        public override string ToString()
        {
            return $"BuyerOrSeller|{Id}|{Name}";
        }
    }


    [Serializable]
    public record Support(UserId Id) : User(Id)
    {
        public override string ToString() => $"Support|{Id}";
    }


    public static User NewBuyerOrSeller(UserId id, string? name)
    {
        return new BuyerOrSeller(id, name);
    }

    public static User NewSupport(UserId id)
    {
        return new Support(id);
    }

    public static bool TryParse(string user, out User? value)
    {
        Match match = new Regex("(?<type>\\w*)\\|(?<id>[^|]*)(\\|(?<name>.*))?").Match(user);
        if (match.Success)
        {
            string typ = match.Groups["type"].Value;
            string id = match.Groups["id"].Value;
            string name = match.Groups["name"].Value;
            switch (typ)
            {
                case "BuyerOrSeller":
                    value = NewBuyerOrSeller(UserId.NewUserId(id), name);
                    return true;
                case "Support":
                    value = NewSupport(UserId.NewUserId(id));
                    return true;
                default:
                    value = default;
                    return false;
            }
        }

        value = default;
        return false;
    }
}