using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

// Library for communication with IBM MQ system
namespace MQ
{

    // QueueManager class - API for communication with QMs
    // It can get the following info:
    // 1. Messaging
    //      1.1 Retrieve all message IDs in a specific queue
    //      1.2 Retrieve message content for specific message ID
    // 2. Queue Manager
    //      2.1 Create connection based on specific URL, username and apikey
    //      2.2 Retrieve general info on QM
    // 3. Queue
    //      3.1 Retrieve information on all queues residing on QM
    //      3.2 Retrieve information on specific queue

    public class QueueManager
    {
        private HttpClient client;
        private readonly string baseUrl;
        private readonly string username;
        private readonly string apikey;
        private readonly string qmgr;


        public QueueManager(string url, string qmgr, string username, string apikey)
        {
            this.baseUrl = url;
            this.qmgr = qmgr;
            this.username = username;
            this.apikey = apikey;

            client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Authenticate();
        }


        // Post request to the Login endpoint, which create cookies for authentication
        public void Authenticate()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/ibmmq/rest/v1/login");
            string body = "{\"username\":\"" + username + "\",\"password\":\"" + apikey + "\"}";
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.SendAsync(request).Result;
        }


        private string GetRequest(string endpoint)
        {
            HttpResponseMessage response = client.GetAsync(endpoint).Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            return responseBody;
        }


        public string GetAllChannels()
        {
            return GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "/channel");
        }


        public string GetAllMessageIds(string queue)
        {
            return GetRequest("/ibmmq/rest/v1/messaging/qmgr/" + qmgr + "/queue/" + queue + "/messagelist");
        }


        public string GetMessageContent(string queue, string messageId)
        {
            return GetRequest("/ibmmq/rest/v1/messaging/qmgr/" + qmgr + "/queue/" + queue + "/message?messageId=" + messageId);
        }


        public string GetQmgr()
        {
            return GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "?attributes=*");
        }


        public string GetAllQueues()
        {
            return GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "/queue?attributes=*");
        }


        public string GetQueue(string queue)
        {
            return GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "/queue/" + queue + "?attributes=*");
        }


    }

    public class QueueManagerTest
    {
        static void Main(string[] args)
        {
            string testUsername = "your-username";
            string testAPIKey = "XXXXXX";
            string testQMUrl = "https://web-qm1-3628.qm.eu-gb.mq.appdomain.cloud:443";
            string testQmgr = "QM1";
            QueueManager mq = new QueueManager(testQMUrl, testQmgr, testUsername, testAPIKey);

            string queueInfo = mq.GetQueue("DEV.QUEUE.1");
            Console.WriteLine("{0}", queueInfo);

        }
    }
}