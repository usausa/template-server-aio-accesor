namespace Template
{
    using System;

    public static class Extensions
    {
        public static DateTime? NextDate(this DateTime? value)
        {
            if (value.HasValue)
            {
                try
                {
                    return value.Value.Date.AddDays(1);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return DateTime.MaxValue;
                }
            }

            return null;
        }
    }
}
