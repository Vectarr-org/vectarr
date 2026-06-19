namespace Vectarr.Palooza.Utils
{
    public static class DateTimeExtensions
    {
        public static DateTime ToUtc(this DateTime dt) => dt.Kind switch
        {
            DateTimeKind.Utc => dt,
            DateTimeKind.Unspecified => DateTime.SpecifyKind(dt, DateTimeKind.Utc),
            _ => dt.ToUniversalTime()
        };
    }
}
