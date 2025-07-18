namespace FutbolYa.WebAPI.Helpers
{
    public static class TimeExtensions
    {
        public static string ToHoraFormato(this DateTime fechaHora)
        {
            return fechaHora.ToString("HH:mm:ss");
        }
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

    }
}
