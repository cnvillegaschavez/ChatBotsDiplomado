using DiplomadoBot.App.Models;
using DiplomadoBot.App.Utilities;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DiplomadoBot.App.Services
{
    public class StoreServices
    {

        public static async Task<OperationResult> CreateReservationAsync(ReservationDto reservation)
        {
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Settings.GetInternalService());
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var resultReservation = await client.PostAsJsonAsync("api/reservations", reservation);
                var resultContent = await resultReservation.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<OperationResult>(resultContent);
                return response;
            }
        }
    }
}