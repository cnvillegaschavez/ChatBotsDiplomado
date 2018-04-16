
using System;

namespace DiplomadoBot.App.Models
{
    [Serializable]
    public class HourQuery
    {
        
        public string Hours { get; set; }

        public static HourQuery Parse(dynamic o)
        {
            try
            {
                return new HourQuery
                {
                    Hours = o.Hours.ToString()
                };
            }
            catch
            {
                throw new InvalidCastException("HourQuery could not be read");
            }
        }
    }
}