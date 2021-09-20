using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TryingHttp
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();
        //static void Main(string[] args)
        //{

        //}
        static async Task Main()
        {
            Console.WriteLine("Welcome to vk links getter v 1.0.0.");
            Console.WriteLine("Dedicated to myself.");
            Console.WriteLine("To exit press Ctrl + C.");
            Console.WriteLine("To continue press Enter.");
            Console.Read();

            List<string> links = new List<string>();

            string url = GetURL();
            string path = GetPath();

            Console.WriteLine("Requesting the links...");

            JObject JSONObject = await GetAlbumLinks(url);

            CheckIfWrongCreds(JSONObject);

            JArray itemsArray = ParseArrayFromJSONString(JSONObject["response"]["items"].ToString());

            foreach (JObject item in itemsArray)
            {
                JArray sizes = ParseArrayFromJSONString(item["sizes"].ToString());
                int length = sizes.Count;
                links.Add("'" + item["sizes"][length - 1]["url"].ToString() + "',");
                Console.WriteLine(item["sizes"][length - 1]["url"]);

            }
            string[] linksToTxt = links.ToArray();
            File.WriteAllLines(path + "/links.txt", linksToTxt);
            Console.Read();
        }
        static void CheckIfWrongCreds(JObject JSON)
        {
            if (JSON["error"] is Object)
            {
                Console.WriteLine(JSON["error"]["error_msg"]);
                System.Environment.Exit(0);
            }
        }
        static async Task<JObject> GetAlbumLinks(string url)
        {
            JObject value = null;
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                value = JObject.Parse(responseBody);

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return value;
        }
        static JArray ParseArrayFromJSONString(string JSONString)
        {
            return JArray.Parse(JSONString);
        }

        static string GetPath()
        {
            string response = "";
            while (response == "")
            {
                response = GetUserResponse("Type the path for the txt to be saved. Example: 'C:/Users/Admin/Desktop'.");
            }

            return response;
        }
        static string GetURL()
        {
            string response = "";
            while (response == "")
            {
                string ownerID = GetUserResponse("Paste in owner_id.");
                string albumID = GetUserResponse("Paste in album_id.");
                string accessToken = GetUserResponse("Paste in access_token.");
                response = $"https://api.vk.com/method/photos.get?owner_id={ownerID}&album_id={albumID}&count=10&access_token={accessToken}&v=5.130";
            }

            return response;
        }
        static string GetUserResponse(string whatToAsk)
        {
            string response = "";
            while (response == "")
            {
                Console.WriteLine(whatToAsk);
                response = Console.ReadLine();
            }
            return response;
        }
    }
}
