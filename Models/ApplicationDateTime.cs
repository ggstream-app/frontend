using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GGStream.Models
{
    public interface IApplicationDateTime
    {
        string TimeZoneId { get; }
        DateTime Now();
    }
    public class ApplicationDateTime : IApplicationDateTime
    {
        private readonly IConfiguration Configuration;

        public ApplicationDateTime(IConfiguration configuration)
        {
            Configuration = configuration;

            TimeZoneId = Configuration.GetValue<string>("TimeZone");
        }

        public string TimeZoneId { get; }

        public DateTime Now()
        {
            return DateTime
                .UtcNow
                .AddMinutes(GetUtcOffsetWithDaylightSaving(TimeZoneId));
        }

        private double GetUtcOffsetWithDaylightSaving(string timezone)
        {
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            var offsetInMins = tzi.BaseUtcOffset.TotalMinutes;
            var currentDay = DateTime.UtcNow.AddMinutes(offsetInMins);
            if (tzi.IsDaylightSavingTime(currentDay))
                offsetInMins += 60;
            return offsetInMins;
        }
    }
}
