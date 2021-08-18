/**
 * The Channel classes are used to parse the JSON reponse from 
 * the MQSC command "DISPLAY CHANNEL"
 * Reference: https://www.ibm.com/docs/en/ibm-mq/9.2?topic=reference-display-channel-display-channel-definition
 */
namespace MQ
{
    public abstract class Channel
    {
        public string channelName;
        public string channelType;
    }

    public class SenderChannel : Channel
    {
        public string transmissionQueueName;
        public string connectionName;
    }

    public class ReceiverChannel : Channel
    {
        
    }

    public class ApplicationChannel: Channel
    {

    }
}


