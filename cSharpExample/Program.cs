using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ConsoleProgram {
    public class RESTQM {
        //This BASE endpoint is for Queue Manager 1
        // private const string BASE = "https://web-qm1-3628.qm.eu-gb.mq.appdomain.cloud:443";
        private string base_url;
        private HttpClient client;
        private string username, apikey;
        private string qmgr;
        //only one client instance (wrapper)
        public RESTQM (string url, string qmgr, string username, string apikey) {
            this.base_url = url;
            this.qmgr = qmgr;
            this.username = username;
            this.apikey = apikey;
            client = new HttpClient();
            client.BaseAddress = new Uri(base_url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Authenticate();
        }

        ~RESTQM() {
            this.client.Dispose();
        }

        public void Authenticate() {
            //Post request to the Login endpoint, body includes username and password(apikey)
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/ibmmq/rest/v1/login");
            string body = "{\"username\":\"" + username + "\",\"password\":\"" + apikey + "\"}";
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.SendAsync(request).Result;
        }

        private string GetRequest(string endpoint) {
            HttpResponseMessage response = client.GetAsync(endpoint).Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            return responseBody;
        }
        public string GetAllChannels() {
            return GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "/channel");
            //parse json here
        }

        public string GetAllMessageIds(string queue) {
            return GetRequest("/ibmmq/rest/v1/messaging/qmgr/" + qmgr + "/queue/" + queue + "/messagelist");
        }

        public string GetMessageContent(string queue, string messageId) {
            return GetRequest("/ibmmq/rest/v1/messaging/qmgr/" + qmgr + "/queue/" + queue + "/message?messageId=" + messageId);
        }

        public string GetQmgr() {
            return GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "?attributes=*");
        }

        public string GetAllQueues() {
            return GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "/queue?attributes=*");
        }

        public string GetQueue(string queue) {
            return GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "/queue/" + queue + "?attributes=*");
        }

        // 1. Messaging
        //      1.1 retrive all message ids in a specific queue
        //      1.2 iterate thru the message ids to retrive message contents
        // 2. Queue Manager
        //      2.1 user-supplied base uri (for QM), username and apikey
        //      2.2 retrive general info on QM
        // 3. Queue
        //      3.1 retrive information on all queues
        //      3.2 retrive information on specific queue (filter by queue name)
    }

    public class MQTest {
        static void Main(string[] args)
        {  

            RESTQM mq = new RESTQM("https://web-qm1-3628.qm.eu-gb.mq.appdomain.cloud:443", "QM1", "shuchengtian", "aCPHZ4ys0Tnn2xQLPsHc6lEz4CTenKmNsyW9q0MoQ0bf");

            string queueInfo = mq.GetQueue("DEV.QUEUE.1");
            Console.WriteLine("{0}", queueInfo);

            //Close client connection
            mq = null;
        }
    }
}