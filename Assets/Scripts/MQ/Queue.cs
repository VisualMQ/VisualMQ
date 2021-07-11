using System;

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

        //TODO: Assign new message fields when message API is ready
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

        //TODO: Assign new message fields when message API is ready
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

