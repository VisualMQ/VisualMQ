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
            client.DefaultRequestHeaders.Add("ibm-mq-rest-csrf-token", "value");

            Authenticate();

        }

        public string GetQueueManagerName() {
            return this.qmgr;
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
                throw new Exception(response.ReasonPhrase);
            }
        }

        private string PostRequest(string endpoint, string jsonPayload)
        {
            StringContent payload = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(endpoint, payload).Result;
            if (response.IsSuccessStatusCode)
            {
                string responseBody = response.Content.ReadAsStringAsync().Result;
                return responseBody;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public List<Channel> GetAllChannels()
        {
            string response = GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "/channel?attributes=*&status=*");
            _ChannelResponseJson channelJson = JsonUtility.FromJson<_ChannelResponseJson>(response);
            List<Channel> channels = Parser.Parse(channelJson);
            return channels;
        }


        public List<Message> GetAllMessages(string queue)
        {
            string response = GetRequest("/ibmmq/rest/v1/messaging/qmgr/" + qmgr + "/queue/" + queue + "/messagelist");
            _MessageResponseJson messageJson = JsonUtility.FromJson<_MessageResponseJson>(response);
            List<Message> messages = Parser.Parse(messageJson);
            return messages;
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

        // Get the queue under current QM
        public Queue GetQueue(string queue)
        {
            string response = GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "/queue/" + queue + "?attributes=*&status=*");
            _QueueResponseJson queueJson = JsonUtility.FromJson<_QueueResponseJson>(response);
            List<Queue> queues = Parser.Parse(queueJson);
            return queues[0];
        }

        // Get the channel under current QM
        public Channel GetChannel(string channel)
        {
            string response = GetRequest("/ibmmq/rest/v1/admin/qmgr/" + qmgr + "/channel/" + channel + "?attributes=*&status=*");
            _ChannelResponseJson channelJson = JsonUtility.FromJson<_ChannelResponseJson>(response);
            List<Channel> channels = Parser.Parse(channelJson);
            return channels[0];
        }

        public Application GetApplication(string application)
        {
            string jsonRequest = "{\"type\":\"runCommandJSON\",\"command\":\"display\",\"qualifier\":\"conn\",\"name\":\"" + application + "\",\"responseParameters\":[\"all\"],\"parameters\":{\"type\":\"*\"}}";
            string response = PostRequest("/ibmmq/rest/v2/admin/action/qmgr/" + qmgr + "/mqsc", jsonRequest);
            _ApplicationResponseJson applicationsJson = JsonUtility.FromJson<_ApplicationResponseJson>(response);
            List<Application> applications = Parser.Parse(applicationsJson);
            return applications[0];
        }

        public List<Application> GetAllApplications()
        {
            string jsonRequest = "{\"type\":\"runCommandJSON\",\"command\":\"display\",\"qualifier\":\"conn\",\"name\":\"*\",\"responseParameters\":[\"all\"],\"parameters\":{\"type\":\"*\"}}";
            string response = PostRequest("/ibmmq/rest/v2/admin/action/qmgr/" + qmgr + "/mqsc", jsonRequest);
            _ApplicationResponseJson applicationsJson = JsonUtility.FromJson<_ApplicationResponseJson>(response);
            List<Application> applications = Parser.Parse(applicationsJson);
            // Filter out all system connections, there are lots of them
            // We can potentially change this in the future
            List<Application> filteredApplications = new List<Application>();
            foreach (Application application in applications)
            {
                if (application.appltype != "SYSTEM")
                {
                    filteredApplications.Add(application);
                }
            }
            return filteredApplications;
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

                qmgr.installationName = qmgrJson.extended.installationName;
                qmgr.permitStandby = qmgrJson.extended.permitStandby;
                qmgr.isDefaultQmgr = qmgrJson.extended.isDefaultQmgr;

                qmgr.publishSubscribeState = qmgrJson.status.publishSubscribeState;
                qmgr.connectionCount = qmgrJson.status.connectionCount;
                qmgr.channelInitiatorState = qmgrJson.status.channelInitiatorState;
                qmgr.ldapConnectionState = qmgrJson.status.ldapConnectionState;
                qmgr.started = qmgrJson.status.started;

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
                            ((TransmissionQueue)queue).currentDepth = queueJson.status.currentDepth;
                            ((TransmissionQueue)queue).openInputCount = queueJson.status.openInputCount;
                            ((TransmissionQueue)queue).openOutputCount = queueJson.status.openOutputCount;
                        }
                        else
                        {
                            queue = new LocalQueue();
                            ((LocalQueue)queue).currentDepth = queueJson.status.currentDepth;
                            ((LocalQueue)queue).openInputCount = queueJson.status.openInputCount;
                            ((LocalQueue)queue).openOutputCount = queueJson.status.openOutputCount;
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

        public static List<Message> Parse(_MessageResponseJson messageResponseJson)
        {
            List<Message> messages = new List<Message>();
            foreach (_MessageJson messageJson in messageResponseJson.messages)
            {
                Message message = new Message();
                message.format = messageJson.format;
                message.messageId = messageJson.messageId;

                messages.Add(message);
            }
            return messages;
        }

        public static List<Channel> Parse(_ChannelResponseJson channelResponseJson)
        {
            List<Channel> channels = new List<Channel>();
            foreach (_ChannelJson channelJson in channelResponseJson.channel)
            {
                Channel channel = null;
                switch (channelJson.type)
                {
                    case "sender":
                        channel = new SenderChannel();
                        ((SenderChannel)channel).transmissionQueueName = channelJson.sender.transmissionQueueName;
                        break;

                    case "receiver":
                        channel = new ReceiverChannel();
                        break;

                    default:
                        continue;
                }
                channel.channelName = channelJson.name;
                channel.channelType = channelJson.type;

                channels.Add(channel);
            }
            return channels;
        }

        public static List<Application> Parse(_ApplicationResponseJson applicationResponseJson)
        {
            List<Application> applications = new List<Application>();
            foreach (_ApplicationJson applicationJson in applicationResponseJson.commandResponse)
            {
                Application application = new Application();
                application.conn = applicationJson.parameters.conn;
                application.channel = applicationJson.parameters.channel;
                application.type = applicationJson.parameters.type;
                application.connopts = applicationJson.parameters.connopts;
                application.conntag = applicationJson.parameters.conntag;
                application.appltype = applicationJson.parameters.appltype;
                application.appldesc = applicationJson.parameters.appldesc;
                application.appltag = applicationJson.parameters.appltag;
                application.conname = applicationJson.parameters.conname;
                application.connectedObjects = new List<Application.ConnectedObject>();

                foreach (_ApplicationObjectJson obj in applicationJson.parameters.objects)
                {
                    Application.ConnectedObject connectedObject = new Application.ConnectedObject();
                    connectedObject.objname = obj.objname;
                    connectedObject.objtype = obj.objtype;
                    connectedObject.hstate = obj.hstate;
                    connectedObject.openopts = obj.openopts;
                    connectedObject.reada = obj.reada;
                    application.connectedObjects.Add(connectedObject);
                }

                applications.Add(application);
            }
            return applications;
        }
    }

    /// 
    /// Below are JSON data representation objects
    /// They are used just for serialising JSON API responses
    /// and then Parser class parses them into our internal data
    /// representation.
    /// 
    [Serializable]
    public class _QueueManagerResponseJson
    {
        public List<_QueueManagerJson> qmgr;
    }

    [Serializable]
    public class _QueueManagerJson
    {
        public string name;
        public string state;
        public _QueueManagerExtendedJson extended;
        public _QueueManagerStatusJson status;
    }

    [Serializable]
    public class _QueueManagerExtendedJson
    {
        public string installationName;
        public string permitStandby;
        public bool isDefaultQmgr;
    }

    [Serializable]
    public class _QueueManagerStatusJson
    {
        public string publishSubscribeState;
        public int connectionCount;
        public string channelInitiatorState;
        public string ldapConnectionState;
        public string started;
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
        public _QueueStatusJson status;
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

    [Serializable]
    public class _QueueStatusJson
    {
        public int currentDepth;
        public int openInputCount;
        public int openOutputCount;
    }

    [Serializable]
    public class _MessageResponseJson
    {
        public List<_MessageJson> messages;
    }

    [Serializable]
    public class _MessageJson
    {
        public string format;
        public string messageId;
    }

    [Serializable]
    public class _ChannelResponseJson
    {
        public List<_ChannelJson> channel;
    }

    [Serializable]
    public class _ChannelJson
    {
        public string type;
        public string name;
        public _ChannelSenderJson sender;
    }

    [Serializable]
    public class _ChannelSenderJson
    {
        public string transmissionQueueName;
    }

    [Serializable]
    public class _ApplicationResponseJson
    {
        public List<_ApplicationJson> commandResponse;
    }

    [Serializable]
    public class _ApplicationJson
    {
        public _ApplicationParametersJson parameters;
    }

    [Serializable]
    public class _ApplicationParametersJson
    {
        public string conn;
        public string channel;
        public string type;
        public string conntag;
        public string conname;
        public List<string> connopts;
        public string appltype;
        public string appldesc;
        public string appltag;
        public List<_ApplicationObjectJson> objects;
    }

    [Serializable]
    public class _ApplicationObjectJson
    {
        public string objname;
        public string objtype;
        public string hstate;
        public string astate;
        public string reada;
        public List<string> openopts;
    }
}
