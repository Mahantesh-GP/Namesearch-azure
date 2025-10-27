using System;

namespace DocumentSummarizer.API.Functions
{
    /// <summary>
    /// Provides helper methods for parsing natural language date filters (e.g., "last week") into
    /// concrete date ranges. This is a stub implementation; extend it to suit your requirements.
    /// </summary>
    public static class DateFilterParser
    {
        /// <summary>
        /// Parses a human‑readable date range description into a tuple of start and end dates.
        /// For example, "last week" may return dates spanning the previous seven days.
        /// </summary>
        /// <param name="filter">A natural language description of the date range.</param>
        /// <returns>A tuple containing the inclusive start date and exclusive end date.</returns>
        public static (DateTimeOffset? from, DateTimeOffset? to) Parse(string? filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return (null, null);
            }

            var today = DateTimeOffset.UtcNow;
            filter = filter.Trim().ToLowerInvariant();
            return filter switch
            {
                "today" => (today.Date, today.Date.AddDays(1)),
                "yesterday" => (today.Date.AddDays(-1), today.Date),
                "last week" => (today.Date.AddDays(-7), today.Date),
                _ => (null, null)
            };
        }
    }
}