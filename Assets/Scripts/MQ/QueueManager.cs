/*
 * The QM classes are used to parse the JSON reponse from 
 * the REST API GET https://host:port/ibmmq/rest/v1/admin/qmgr/{qmgrName}
 * Reference: https://www.ibm.com/docs/en/ibm-mq/9.0?topic=adminqmgr-get
 * It also holds the queue/channel/application objects that lives on the QM
 */
using System.Collections.Generic;

namespace MQ
{
    public class QueueManager
    {
        public string qmgrName;
        public string state;

        //populated by the State object
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


