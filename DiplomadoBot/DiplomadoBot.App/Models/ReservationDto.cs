using System;

namespace DiplomadoBot.App.Models
{
    public class ReservationDto
    {
        public Guid ReservationId { get; set; }
        public Guid ServiceId { get; set; }
        public string CustomerName { get; set; }
        public string Hour { get; set; }
        public int Day { get; set; }
        public string Telephone { get; set; }

        public static ReservationDto Parse(dynamic o)
        {
            try
            {
                return new ReservationDto
                {
                    ReservationId = Guid.NewGuid(),
                    ServiceId = Guid.Parse("2aaa0c73-4b1e-4b2e-b55d-390a16c8acda"),
                    CustomerName = o.name.ToString(),
                    Day = 1,
                    Telephone = o.telephone.ToString()

                };
            }
            catch
            {
                throw new InvalidCastException("ReservationDto could not be read");
            }
        }
    }
}