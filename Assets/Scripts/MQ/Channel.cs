using System;
using System.Collections.Generic;


namespace MQ
{
    public abstract class Channel
    {

        public string channelName;
        public string channelType;
    }

    public class SenderChannel : Channel
    {

    }

    public class ReceiverChannel : Channel
    {

    }
}


