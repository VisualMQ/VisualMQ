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

}

