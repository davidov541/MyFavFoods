namespace MyFavFoods.YelpAPI.Objects
{
    public enum ResponseCodes
    {
        OK = 0,
        ServerError = 1,
        InvalidAPIKey = 2,
        MissingAPIKey = 3,
        RequestLimitExceeded = 4,
        APINotAvailable = 5,
        RequestNotUnderstood = 6,
        UnspecifiedLocation = 200,
        BadTermParameter = 201,
        BadLocationParameter = 202,
        AreaToLarge = 203,
        UnknownCategory = 204
    }
}
