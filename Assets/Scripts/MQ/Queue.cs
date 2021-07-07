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

    }


    public class RemoteQueue : Queue
    {
        public string targetQueueName;
        public string targetQmgrName;
        public string transmissionQueueName;

    }


    public class TransmissionQueue : Queue
    {
        public int currentDepth;
        //TODO: Assign new message fields when message API is ready
    }


    public class AliasQueue : Queue
    {
        public string targetQueueName;
    }


    public class LocalQueue : Queue
    {
        public int currentDepth;
        //TODO: Assign new message fields when message API is ready
    }

}

