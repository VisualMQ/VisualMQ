using System;
using System.Collections.Generic;


namespace MQ
{
    public class QueueManager
    {
        public string qmgrName;
        public string state;

        //populated separatedly
        public List<Queue> queues;
        public List<Channel> channels;
        public List<Application> applications;

        //extended
        public string installationName;
        public string permitStandby;
        public bool isDefaultQmgr;

        //status
        public string publishSubscribeState;
        public int connectionCount;
        public string channelInitiatorState;
        public string ldapConnectionState;
        public string started;
    }
}


