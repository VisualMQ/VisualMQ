using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

// Library for communication with IBM MQ system
namespace MQ
{
    // QMClient class - API for communication with QMs
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

    public class QMClient
    {
        private HttpClient client;
        private readonly string baseUrl;
        private readonly string username;
        private readonly string apikey;
        private readonly string qmgr;

        public QMClient(string url, string qmgr, string username, string apikey)
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
            if (!response.IsSuccessStatusCode) { //Invalid credentials or API endpoints
                throw new Exception();
            }
        }

        private string GetRequest(string endpoint)
        {
            HttpResponseMessage response = client.GetAsync(endpoint).Result;
            if (response.IsSuccessStatusCode) {
                string responseBody = response.Content.ReadAsStringAsync().Result;
                return responseBody;
            } else {
                throw new Exception();
            }

        }

        public string GetAllChannels()
        {
            return GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "/channel");
        }


        public string GetAllMessageIds(string queue)
        {
            try {
                return GetRequest("/ibmmq/rest/v1/messaging/qmgr/" + qmgr + "/queue/" + queue + "/messagelist");
            } catch (Exception) {
                return "Not supported for this type of queue.";
            }
            
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
            return GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "/queue?name=DEV*&attributes=*&status=*");
        }


        public string GetQueue(string queue)
        {
            return GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "/queue/" + queue + "?attributes=*");
        }
        
    }

    //////Below are classes used for internal state representation

    public class Queue
    {
        public String name {get; set;}
        public String type {get; set;}
        public QueueRemote remote {get; set;}
        public QueueStorage storage {get; set;}
        public MessagesInfo messages {get; set;}
        
    }
    public class QueuesFactory //TODO: 1.parse extended attributes
    {
        public Dictionary<string, object>[] queue {get; set;}

        public QueuesFactory() {}

        public List<Queue> makeQueues(string info)
        {
            List<Queue> Queues = new List<Queue>();
            QueuesFactory t = new QueuesFactory();
            t = JsonConvert.DeserializeObject<QueuesFactory>(info);

            for (int i = 0; i < t.queue.Length; i++) {
                Queue q = new Queue();
                q.name = t.queue[i]["name"].ToString();
                q.type = t.queue[i]["type"].ToString();

                //storage
                if (t.queue[i].ContainsKey("storage")) {
                    string storageFields = t.queue[i]["storage"].ToString();
                    Dictionary<string, string> stor = JsonConvert.DeserializeObject<Dictionary<string, string>>(storageFields);
                    QueueStorage tt = new QueueStorage();
                    tt.maximumDepth = stor["maximumDepth"];
                    q.storage = tt;
                } else { //some queues doesn't have 'storage' attribute, so maximum depth is set to 0
                    QueueStorage tt = new QueueStorage();
                    tt.maximumDepth = "0";
                    q.storage = tt;
                }

                //remote
                if (t.queue[i].ContainsKey("remote")) {
                    string remoteFields = t.queue[i]["remote"].ToString();
                    Dictionary<string, string> remo = JsonConvert.DeserializeObject<Dictionary<string, string>>(remoteFields);
                    QueueRemote tt = new QueueRemote();
                    tt.queueName = remo["queueName"];
                    tt.transmissionQueueName = remo["transmissionQueueName"];
                    tt.qmgrName = remo["qmgrName"];
                    q.remote = tt;
                } else { //local queues doesn't have 'remote' attribute
                    q.remote = null;
                }
                //add queue to Queues
                Queues.Add(q);
            }
            return Queues;
        }
    }

    public class QueueRemote //Remote queue info (QM, mapping, etc)
    {
        public string queueName {get; set;}
        public string transmissionQueueName {get; set;}
        public string qmgrName {get; set;}

    }

    public class QueueStorage
    {
        public string maximumDepth {get; set;}
    }

    public class QMInfo 
    {
        public Dictionary<string, object>[] qmgr {get; set;}
        public string name {get; set;}
        public string state {get; set;}
        public QMExtended extended {get; set;}

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

    ///// Internal QueueManager Representation
    public class QueueManager
    {
        public QMInfo QMInfo {get; set;}
        public List<Queue> Queues {get; set;}
        // public List<MessagesInfo> MessagesInfo {get; set;}

        public QueueManager(QMInfo QMInfo, List<Queue> Queues)
        {
            this.QMInfo = QMInfo;
            this.Queues = Queues;
            // this.MessagesInfo = MessagesInfo;
        }
    }
    ///// Internal State Representation

    public class State
    {
        public List<QueueManager> QMs {get; set;}
        //TODO: add channels
        public State(List<QueueManager> QMs)
        {
            this.QMs = QMs;
        }
    }

    public class QueueManagerTest
    {
        static void Main(string[] args)
        {
            //Step one: create an Http client (QMClient class), remember to set the username and apikey
            string testUsername = "shuchengtian";
            string testAPIKey = "aCPHZ4ys0Tnn2xQLPsHc6lEz4CTenKmNsyW9q0MoQ0bf";
            string testQMUrl = "https://web-qm1-3628.qm.eu-gb.mq.appdomain.cloud:443";
            string testQmgr = "QM1";

            //Step one+: create a second Http client
            QMClient mq, mq2;
            string testQMUrl2 = "https://web-qm2-3628.qm.eu-gb.mq.appdomain.cloud:443";
            string testQmgr2 = "qm2";
            try {
                mq = new QMClient(testQMUrl, testQmgr, testUsername, testAPIKey);
                mq2 = new QMClient(testQMUrl2, testQmgr2, testUsername, testAPIKey);
            } catch (Exception) {
                Console.WriteLine("Authentication failed.");
                return;
            }

            //Step two: create QMInfo, QueuesFactory(for making queues), and inset MessagesInfo objects to each queue
            string QMInfo = mq.GetQmgr();
            QMInfo QM1 = new QMInfo(QMInfo);

            string QMInfo2 = mq2.GetQmgr();
            QMInfo QM2 = new QMInfo(QMInfo2);

            string allqueue = mq.GetAllQueues();
            QueuesFactory Qs1 = new QueuesFactory();
            List<Queue> Queues1 = Qs1.makeQueues(allqueue);

            string allqueue2 = mq2.GetAllQueues();
            QueuesFactory Qs2 = new QueuesFactory();
            List<Queue> Queues2 = Qs2.makeQueues(allqueue2);

            for (int i = 0; i < Queues1.Count; i++) {
                string messageInfo = mq.GetAllMessageIds(Queues1[i].name);
                Queues1[i].messages = new MessagesInfo(Queues1[i].name, messageInfo);
            }

            for (int i = 0; i < Queues2.Count; i++) {
                string messageInfo = mq2.GetAllMessageIds(Queues2[i].name);
                Queues2[i].messages = new MessagesInfo(Queues2[i].name, messageInfo);
            }
            
            //Step three: create a QueueManager object that consists QMinfo and list of queues
            //Maybe store the QMClient in the QueueManager Object as well?
            QueueManager QueueManager1 = new QueueManager(QM1, Queues1);
            QueueManager QueueManager2 = new QueueManager(QM2, Queues2);
            
            //Step four: create a list of QueueManagers and create an internal state object
            List<QueueManager> qms = new List<QueueManager>();
            qms.Add(QueueManager1);
            qms.Add(QueueManager2);
            State internalState = new State(qms);
        }
    }
}