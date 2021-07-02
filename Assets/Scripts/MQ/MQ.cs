using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEngine;


namespace MQ
{

    // Library for communication with IBM MQ system
    public class Client
    {
        private HttpClient client;
        private readonly string baseUrl;
        private readonly string username;
        private readonly string apikey;
        private readonly string qmgr;

        public Client(string url, string qmgr, string username, string apikey)
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
            if (!response.IsSuccessStatusCode)
            { //Invalid credentials or API endpoints
                throw new Exception();
            }
        }

        private string GetRequest(string endpoint)
        {
            HttpResponseMessage response = client.GetAsync(endpoint).Result;
            if (response.IsSuccessStatusCode)
            {
                string responseBody = response.Content.ReadAsStringAsync().Result;
                return responseBody;
            }
            else
            {
                throw new Exception();
            }

        }

        public string GetAllChannels()
        {
            return GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "/channel");
        }


        public string GetAllMessageIds(string queue)
        {
            try
            {
                return GetRequest("/ibmmq/rest/v1/messaging/qmgr/" + qmgr + "/queue/" + queue + "/messagelist");
            }
            catch (Exception)
            {
                return "Not supported for this type of queue.";
            }

        }


        public string GetMessageContent(string queue, string messageId)
        {
            return GetRequest("/ibmmq/rest/v1/messaging/qmgr/" + qmgr + "/queue/" + queue + "/message?messageId=" + messageId);
        }


        public QueueManager GetQmgr()
        {
            string response = GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "?attributes=*");
            _QueueManagerResponseJson qmgrJson = JsonUtility.FromJson<_QueueManagerResponseJson>(response);
            List<QueueManager> qmgrs = Parser.Parse(qmgrJson);
            return qmgrs[0];
        }


        public List<Queue> GetAllQueues()
        {
            string response = GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "/queue?name=DEV*&attributes=*&status=*");
            _QueueResponseJson queueJson = JsonUtility.FromJson<_QueueResponseJson>(response);
            List<Queue> queues = Parser.Parse(queueJson);
            return queues;
        }


        public Queue GetQueue(string queue)
        {
            string response = GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "/queue/" + queue + "?attributes=*");
            _QueueResponseJson queueJson = JsonUtility.FromJson<_QueueResponseJson>(response);
            List<Queue> queues = Parser.Parse(queueJson);
            return queues[0];
        }

    }



    /// 
    /// Parses class for converting JSON representation to our internal data model
    /// 
    public class Parser
    {

        public static List<QueueManager> Parse(_QueueManagerResponseJson qmgrResponseJson)
        {
            List<QueueManager> qmgrs = new List<QueueManager>();
            foreach (_QueueManagerJson qmgrJson in qmgrResponseJson.qmgr)
            {
                QueueManager qmgr = new QueueManager();
                qmgr.qmgrName = qmgrJson.name;
                qmgr.state = qmgrJson.state;
                qmgr.instVer = qmgrJson.instVer;

                qmgrs.Add(qmgr);
            }
            return qmgrs;
        }

        public static List<Queue> Parse(_QueueResponseJson queueResponseJson)
        {
            List<Queue> queues = new List<Queue>();
            foreach (_QueueJson queueJson in queueResponseJson.queue)
            {
                Queue queue = null;
                switch (queueJson.type)
                {
                    case "alias":
                        queue = new AliasQueue();
                        ((AliasQueue)queue).targetQueueName = queueJson.alias.targetName;
                        break;

                    case "remote":
                        queue = new RemoteQueue();
                        ((RemoteQueue)queue).targetQueueName = queueJson.remote.queueName;
                        ((RemoteQueue)queue).targetQmgrName = queueJson.remote.qmgrName;
                        ((RemoteQueue)queue).transmissionQueueName = queueJson.remote.transmissionQueueName;
                        break;

                    case "local":
                        if (queueJson.general.isTransmissionQueue)
                        {
                            queue = new TransmissionQueue();
                        }
                        else
                        {
                            queue = new LocalQueue();
                        }
                        break;

                    default:
                        break;
                }
                // Add properties that are the same for all queues
                queue.queueName = queueJson.name;
                queue.maxNumberOfMessages = queueJson.storage.maximumDepth;
                queue.maxMessageLength = queueJson.storage.maximumMessageLength;
                queue.inhibitGet = queueJson.general.inhibitGet;
                queue.inhibitPut = queueJson.general.inhibitPut;
                queue.description = queueJson.general.description;
                queue.timeCreated = queueJson.timestamps.created;
                queue.timeAltered = queueJson.timestamps.altered;

                queues.Add(queue);
            }
            return queues;
        }

    }


    /// 
    /// These are JSON data representation
    /// 
    [Serializable]
    public class _QueueManagerResponseJson
    {
        public List<_QueueManagerJson> qmgr;
    }

    [Serializable]
    public class _QueueManagerJson
    {
        public string instVer;
        public string name;
        public string state;
    }

    [Serializable]
    public class _QueueResponseJson
    {
        public List<_QueueJson> queue;
    }

    [Serializable]
    public class _QueueJson
    {
        public string name;
        public string type;

        public _QueueTimestampsJson timestamps;
        public _QueueStorageJson storage;
        public _QueueRemoteJson remote;
        public _QueueAliasJson alias;
        public _QueueGeneralJson general;
    }

    [Serializable]
    public class _QueueAliasJson
    {
        public string targetName;
        public string targetType;
    }

    [Serializable]
    public class _QueueRemoteJson
    {
        public string queueName;
        public string qmgrName;
        public string transmissionQueueName;
    }

    [Serializable]
    public class _QueueGeneralJson
    {
        public bool inhibitPut;
        public bool inhibitGet;
        public bool isTransmissionQueue;
        public string description;
    }

    [Serializable]
    public class _QueueTimestampsJson
    {
        public string created;
        public string clustered;
        public string altered;
    }

    [Serializable]
    public class _QueueStorageJson
    {
        public int maximumDepth;
        public int maximumMessageLength;
    }




    /// 
    /// Internal data representation
    /// 



    public class RemoteQueue : Queue
    {
        public string targetQueueName;
        public string targetQmgrName;
        public string transmissionQueueName;

    }

    public class TransmissionQueue : Queue
    {

    }

    public class AliasQueue : Queue
    {
        public string targetQueueName;
    }

    public class LocalQueue : Queue
    {

    }



    public class MessagesInfo //TODO: improve error handling in API and Parsing
    {
        public Dictionary<string, string>[] messages { get; set; }
        public string name { get; set; }
        public MessagesInfo() { }
        public MessagesInfo(string name, string info)
        {
            //MessagesInfo t = new MessagesInfo();
            //try { 
            //    t = JsonConvert.DeserializeObject<MessagesInfo>(info);
            //    this.messages = t.messages;
            //} catch (Exception) {
            //}

            this.name = name;
        }
    }


}
