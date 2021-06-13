using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ConsoleProgram {
    public class RESTMQ {
        //This BASE endpoint is for Queue Manager 1
        private const string BASE = "https://web-qm1-3628.qm.eu-gb.mq.appdomain.cloud:443/ibmmq/rest/v1";
        
        public static HttpClient Authenticate(string username, string apikey) {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(BASE);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Post request to the Login endpoint, body includes username and password(apikey)
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/login");
            string body = "{\"username\":\"" + username + "\",\"password\":\"" + apikey + "\"}";
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
  
            HttpResponseMessage response = client.SendAsync(request).Result;
            return client;
        }

        public static string GetAllChannels(ref HttpClient client, string qmgr) {
            HttpResponseMessage response = client.GetAsync("/admin/qmgr/" + qmgr + "/channel").Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            return responseBody;
        }
    }

    public class MQTest {
        private const string QM1 = "QM1";
        static void Main(string[] args)
        {  
            //First Authenticate with the IBM MQ on Cloud
            HttpClient client = RESTMQ.Authenticate("shuchengtian", "aCPHZ4ys0Tnn2xQLPsHc6lEz4CTenKmNsyW9q0MoQ0bf"); //hard-coded for now

            //Perform any Rest API calls below
            // string all_channels = RESTMQ.GetAllChannels(ref client, QM1);
            // Console.WriteLine("{0}", all_channels);

            HttpResponseMessage response = client.GetAsync("/admin/qmgr/QM1/channel").Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine("{0}", responseBody);

            //Close client connection
            client.Dispose();
        }
    }
}