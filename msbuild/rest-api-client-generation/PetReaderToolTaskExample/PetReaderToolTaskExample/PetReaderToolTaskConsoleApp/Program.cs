using System;
using System.Net.Http;

namespace PetReaderToolTaskConsoleApp
{
    internal class Program
    {
        private const string baseUrl = "https://petstore.swagger.io/v2";
        static void Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseUrl);
            var petClient = new PetRestApiClient.PetRestApiClient(httpClient);
            var pet = petClient.GetPetByIdAsync(1).Result;
            Console.WriteLine($"Id: {pet.Id} Name: {pet.Name} Status: {pet.Status} CategoryName: {pet.Category.Name}");
        }
    }
}
