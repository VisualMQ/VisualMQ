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
        public Boolean holdsMessages;
        public int currentDepth;

    }


    public class RemoteQueue : Queue
    {
        public string targetQueueName;
        public string targetQmgrName;
        public string transmissionQueueName;
        public new const int currentDepth = 0;

    }


    public class TransmissionQueue : Queue
    {
        //TODO: Assign new message fields when message API is ready
    }


    public class AliasQueue : Queue
    {
        public string targetQueueName;
        public new const int currentDepth = 0;
    }


    public class LocalQueue : Queue
    {
        //TODO: Assign new message fields when message API is ready
    }

}

