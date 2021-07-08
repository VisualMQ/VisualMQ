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

    }


    public class AliasQueue : Queue
    {
        public string targetQueueName;
    }


    public class LocalQueue : Queue
    {

    }

}

