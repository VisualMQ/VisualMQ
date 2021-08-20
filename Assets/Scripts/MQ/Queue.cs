/*
 * The Queue classes are used to parse the JSON reponse from 
 * the REST API GET https://host:port/ibmmq/rest/v1/admin/qmgr/{qmgrName}/queue/{queueName}
 * Reference: https://www.ibm.com/docs/en/ibm-mq/9.0?topic=adminqmgrqmgrnamequeue-get
 */
using System.Collections.Generic;

namespace MQ
{
    public abstract class Queue
    {
        public string queueName;
        public int maxNumberOfMessages;
        public int maxMessageLength;
        public bool inhibitPut;
        public bool inhibitGet;
        public string description;
        public string timeCreated;
        public string timeAltered;
        public int currentDepth;
        public bool holdsMessages{ get; protected set; }

        public List<Message> messages;

        public static string typeName { get; protected set; }
        public abstract string GetTypeName();
    }

    
    public class RemoteQueue : Queue
    {
        public string targetQueueName;
        public string targetQmgrName;
        public string transmissionQueueName;

        public new static string typeName = "Remote";

        public RemoteQueue()
        {
            currentDepth = 0;
            holdsMessages = false;
        }
        
        public override string GetTypeName()
        {
            return RemoteQueue.typeName;
        }

    }


    public class TransmissionQueue : Queue
    {
        public new static string typeName = "Transmission";
        public int openInputCount;
        public int openOutputCount;

        public TransmissionQueue()
        {
            holdsMessages = true;
        }

        public override string GetTypeName()
        {
            return TransmissionQueue.typeName;
        }
    }

    public class AliasQueue : Queue
    {
        public new static string typeName = "Alias";

        public string targetQueueName;

        public AliasQueue()
        {
            currentDepth = 0;
            holdsMessages = false;
        }

        public override string GetTypeName()
        {
            return AliasQueue.typeName;
        }
    }


    public class LocalQueue : Queue
    {
        public new static string typeName = "Local";
        public int openInputCount;
        public int openOutputCount;

        public LocalQueue()
        {
            holdsMessages = true;
        }


        public override string GetTypeName()
        {
            return LocalQueue.typeName;
        }

    }

}

