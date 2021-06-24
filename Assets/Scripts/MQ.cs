using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using UnityEngine;

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
            if (response.IsSuccessStatusCode) {
                string responseBody = response.Content.ReadAsStringAsync().Result;
                return responseBody;
            } else {
                return "Bad Request";
            }

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


        public string GetAllQueues() //Only DEV queues.
        {
            return GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "/queue?name=DEV.*&attributes=storage");
        }


        public string GetQueue(string queue)
        {
            return GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "/queue/" + queue + "?attributes=*");
        }
        
    }


    //////Below are classes used for internal state representation
    public class QueuesInfo //TODO: 1. make a list of queues? 2.parse extended attributes
    {
        public Dictionary<string, object>[] queue {get; set;}
        public List<String> name {get; set;}
        public List<String> type {get; set;}
        public QueueRemote remote {get; set;}
        public List<QueueStorage> storage{get; set;}

        /*public GameObject obj; TODO WHERE TO ADD THIS*/

        public QueuesInfo() {}

        public QueuesInfo(string info)
        {
            QueuesInfo t = new QueuesInfo();
            t = JsonConvert.DeserializeObject<QueuesInfo>(info);

            this.name = new List<string>();
            this.type = new List<string>();
            this.storage = new List<QueueStorage>();

            for (int i = 0; i < t.queue.Length; i++) {
                this.name.Add(t.queue[i]["name"].ToString());
                this.type.Add(t.queue[i]["type"].ToString());

                //storage
                if (t.queue[i].ContainsKey("storage")) {
                    string storageFields = t.queue[i]["storage"].ToString();
                    Dictionary<string, string> stor = JsonConvert.DeserializeObject<Dictionary<string, string>>(storageFields);
                    QueueStorage tt = new QueueStorage();
                    tt.maximumDepth = stor["maximumDepth"];
                    this.storage.Add(tt);
                } else {
                    QueueStorage tt = new QueueStorage();
                    tt.maximumDepth = "0";
                    this.storage.Add(tt);
                }

            }

        }
    }

    public class QueueRemote //Remote queue info (QM, mapping, etc)
    {

    }

    public class QueueStorage
    {
        public GameObject obj;
        public string maximumDepth {get; set;}
    }

    public class QMInfo 
    {
        public Dictionary<string, object>[] qmgr {get; set;}
        public string name {get; set;}
        public string state {get; set;}
        public QMExtended extended {get; set;}

        public GameObject obj;
        public QMInfo() {}
        public QMInfo(string info)
        {
            QMInfo t = new QMInfo();
            t = JsonConvert.DeserializeObject<QMInfo>(info);
            this.name = t.qmgr[0]["name"].ToString();
            this.state = t.qmgr[0]["state"].ToString();

            //extended fields
            string extendedFields = t.qmgr[0]["extended"].ToString();
            Dictionary<string, string> ext = JsonConvert.DeserializeObject<Dictionary<string, string>>(extendedFields);
            QMExtended tt = new QMExtended();
            tt.installationName = ext["installationName"];
            tt.permitStandby = ext["permitStandby"];
            tt.isDefaultQmgr = ext["isDefaultQmgr"];

            this.extended = tt;
        }
    }

    public class QMExtended
    {
        public string installationName {get; set;}
        public string permitStandby {get; set;}
        public string isDefaultQmgr {get; set;}
    }

    public class MessagesInfo //TODO: improve error handling in API and Parsing
    {
        public Dictionary<string, string>[] messages {get; set;}
        public string name {get; set;}
        public MessagesInfo(){}        
        public MessagesInfo(string name, string info)
        {
            MessagesInfo t = new MessagesInfo();
            try { 
                t = JsonConvert.DeserializeObject<MessagesInfo>(info);
                this.messages = t.messages;
            } catch (Exception) {
            }
            
            this.name = name;
        }
    }

    ///// Internal State Representation
    public class State
    {
        public QMInfo QMInfo {get; set;}
        public QueuesInfo QueuesInfo {get; set;}
        public List<MessagesInfo> MessagesInfo {get; set;}

        public State(QMInfo QMInfo, QueuesInfo QueuesInfo, List<MessagesInfo> MessagesInfo)
        {
            this.QMInfo = QMInfo;
            this.QueuesInfo = QueuesInfo;
            this.MessagesInfo = MessagesInfo;
        }
    }
    /////

    public class QueueManagerTest
    {
        static void Main(string[] args)
        {
            //Step one: create an Http client (QueueManager class), remember to set the username and apikey
            string testUsername = "REPLACE WITH YOUR USERNAME";
            string testAPIKey = "REPLACE WITH YOUR API KEY";
            string testQMUrl = "https://web-qm1-3628.qm.eu-gb.mq.appdomain.cloud:443";
            string testQmgr = "QM1";
            QueueManager mq = new QueueManager(testQMUrl, testQmgr, testUsername, testAPIKey);

            //Step two: create QMInfo, QueuesInfo, and List of MessagesInfo objects
            string QMInfo = mq.GetQmgr();
            QMInfo QM1 = new QMInfo(QMInfo);

            string allqueue = mq.GetAllQueues();
            QueuesInfo Qs1 = new QueuesInfo(allqueue);

            List<MessagesInfo> allQMessages = new List<MessagesInfo>();
            for (int i = 0; i < Qs1.name.Count; i++) {
                string messageInfo = mq.GetAllMessageIds(Qs1.name[i]);
                allQMessages.Add(new MessagesInfo(Qs1.name[i], messageInfo));
            }
            
            //Step three: create a state object that consists of the three objects/list of objects in step three
            State internalState = new State(QM1, Qs1, allQMessages);

            //Example of how to accesss the fields/information
            internalState.QueuesInfo.name.ForEach(Console.WriteLine); //Names of all queues in this queue manageer
            Console.WriteLine("");
            internalState.QueuesInfo.type.ForEach(Console.WriteLine); //Type of all queues in this queue manageer
            Console.WriteLine("");
            internalState.QueuesInfo.storage.ForEach(n => Console.WriteLine(n.maximumDepth)); //Depth of all queues in this queue manageer
            Console.WriteLine("");

            for (int i = 0 ; i < internalState.MessagesInfo.Count; i++){
                if (internalState.MessagesInfo[i].messages != null) { //right now, it is essential to check for null references
                    Console.WriteLine(internalState.MessagesInfo[i].messages.Length); //Current number of messages in each queue
                } else {
                    Console.WriteLine(-1); //If the messages fiels is null, then the queue is a remote queue
                }  
            }
        }
    }
}