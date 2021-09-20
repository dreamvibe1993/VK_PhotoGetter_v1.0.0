using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
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
            Console.WriteLine("\nWelcome to vk links getter v 1.0.0.\n");
            Console.WriteLine("\nTo exit press Ctrl + C.\n");

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

            Console.WriteLine("Success! Press enter to... exit...");
            Console.WriteLine("\nWas coded by lord_extraneous (c) 2021.\n");
            Console.Read();
            System.Environment.Exit(0);
        }
        static void CheckIfWrongCreds(JObject JSON)
        {
            if (JSON["error"] is Object)
            {
                Console.WriteLine(JSON["error"]["error_msg"]);
                Console.Read();
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
            string response = GetUserString("Type the path for the txt to be saved. Example: 'C:/Users/Admin/Desktop'.");

            while (!Regex.IsMatch(response, @"^[A-Z]:"))
            {
                response = GetUserString("I guess you typed wrong path. Try again. Example: 'C:/Users/Admin/Desktop'.");
            }


            return response;
        }
        static string GetURL()
        {
            string response = "";
            while (response == "")
            {
                Console.WriteLine("\nYou may get help here:  https://vk.com/dev/photos.get \n");
                string ownerID = GetUserString("Paste in owner_id.");
                string albumID = GetUserString("Paste in album_id.");
                string accessToken = GetUserString("Paste in access_token.");
                string linksNumber = GetUserInt("Type the number of links you need. (Must be between 1 and 1000)");

                response = $"https://api.vk.com/method/photos.get?owner_id={ownerID}&album_id={albumID}&count={linksNumber}&access_token={accessToken}&v=5.130";
            }

            return response;
        }
        static string GetUserInt(string whatToAsk)
        {
            string response = GetUserString(whatToAsk);
            string resultString = Regex.Match(response, @"\d+").Value;

            while (resultString == "")
            {
                string askedAgain = GetUserString($"You typed {response}. There was no numbers. Try again.");
                resultString = Regex.Match(askedAgain, @"\d+").Value;
            }


            int linksCount = Int32.Parse(resultString);
            if (linksCount < 1) resultString = "0";
            if (linksCount > 1000) resultString = "1000";
            return resultString;
        }
        static string GetUserString(string whatToAsk)
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
