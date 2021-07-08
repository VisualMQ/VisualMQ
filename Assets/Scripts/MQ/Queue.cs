using System;

namespace MQ
{

    public class Queue
    {
        public string queueName;
        public int maxNumberOfMessages;
        public int maxMessageLength;
        public bool inhibitPut;
        public bool inhibitGet;
        public string description;
        public string timeCreated;
        public string timeAltered;
        public bool holdsMessages{ get; protected set; }
        public int currentDepth;
    }


    public class RemoteQueue : Queue
    {
        public string targetQueueName;
        public string targetQmgrName;
        public string transmissionQueueName;
        public RemoteQueue()
        {
            currentDepth = 0;
            holdsMessages = false;
        }

    }


    public class TransmissionQueue : Queue
    {
        //TODO: Assign new message fields when message API is ready
        public TransmissionQueue()
        {
            holdsMessages = true;
        }
    }


    public class AliasQueue : Queue
    {
        public string targetQueueName;
        public AliasQueue()
        {
            currentDepth = 0;
            holdsMessages = false;
        }
    }


    public class LocalQueue : Queue
    {
        //TODO: Assign new message fields when message API is ready
        public LocalQueue()
        {
            holdsMessages = true;
        }
    }

}

