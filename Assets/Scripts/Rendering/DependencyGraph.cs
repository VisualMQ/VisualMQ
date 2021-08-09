using System;
using System.Collections.Generic;
using UnityEngine;

public class DependencyGraph
{
    public Dictionary<string, List<string>> directDependencies; //TODO: change to private with getter method
    public Dictionary<string, List<string>> indirectDependencies;
    public Dictionary<string, List<string>> implicitDependencies;

    private const string QM_NAME_DELIMITER = ".";

    public DependencyGraph()
    {
        directDependencies = new Dictionary<string, List<string>>();
        indirectDependencies = new Dictionary<string, List<string>>();
        implicitDependencies = new Dictionary<string, List<string>>();
    }

    public void CreateDependencyGraph (List<MQ.Queue> queues, List<MQ.Channel> channels, List<MQ.Application> applications, string qmgr)
    {

        foreach (MQ.Queue queue in queues)
        {
            if (queue is MQ.AliasQueue)
            {
                // Direct dependency of the alias queue
                List<string> directDependency = new List<string>();
                directDependency.Add(qmgr + QM_NAME_DELIMITER + ((MQ.AliasQueue)queue).targetQueueName);
                AddDependency(directDependencies, qmgr, queue.queueName, directDependency);

                // Indirect dependency for the target queue of the alias queue
                List<string> indirectDependency = new List<string>();
                indirectDependency.Add(qmgr + QM_NAME_DELIMITER + queue.queueName);
                AddDependency(indirectDependencies, qmgr, ((MQ.AliasQueue)queue).targetQueueName, indirectDependency);
            }


            if (queue is MQ.RemoteQueue)
            {
                // Direct dependency of the remote queue
                List<string> directDependency = new List<string>();
                directDependency.Add(qmgr + QM_NAME_DELIMITER + ((MQ.RemoteQueue)queue).transmissionQueueName);
                directDependency.Add(((MQ.RemoteQueue)queue).targetQmgrName + QM_NAME_DELIMITER + ((MQ.RemoteQueue)queue).targetQueueName);
                AddDependency(directDependencies, qmgr, queue.queueName, directDependency);

                // Indirect dependency for the transmission queue and target queue of the remote queue
                List<string> indirectDependencyTrans = new List<string>();
                indirectDependencyTrans.Add(qmgr + QM_NAME_DELIMITER + queue.queueName);
                AddDependency(indirectDependencies, qmgr, ((MQ.RemoteQueue)queue).transmissionQueueName, indirectDependencyTrans);

                List<string> indirectDependencyTarget = new List<string>();
                indirectDependencyTarget.Add(qmgr + QM_NAME_DELIMITER + queue.queueName);
                AddDependency(indirectDependencies, ((MQ.RemoteQueue)queue).targetQmgrName, ((MQ.RemoteQueue)queue).targetQueueName, indirectDependencyTarget);
            }
        }


        foreach (MQ.Channel channel in channels)
        {
            if (channel is MQ.SenderChannel)
            {
                // Direct dependency of sender channel
                List<string> directDependency = new List<string>();
                directDependency.Add(qmgr + QM_NAME_DELIMITER + ((MQ.SenderChannel)channel).transmissionQueueName); //transmission queue
                AddDependency(directDependencies, qmgr, channel.channelName, directDependency);

                // Indirect dependency for the transmission queue of the sender channel
                List<string> indirectDependency1 = new List<string>();
                indirectDependency1.Add(qmgr + QM_NAME_DELIMITER + channel.channelName);
                AddDependency(indirectDependencies, qmgr, ((MQ.SenderChannel)channel).transmissionQueueName, indirectDependency1);

                if (channel.channelName == "QM1.QM2")
                {
                    List<string> indirectDependency2 = new List<string>();
                    indirectDependency2.Add(qmgr + QM_NAME_DELIMITER + channel.channelName);
                    AddDependency(indirectDependencies, "qm2", channel.channelName, indirectDependency2);
                }


            }
            else if (channel is MQ.ReceiverChannel)
            {
                if (channel.channelName == "QM1.QM2")
                {
                    List<string> indirectDependency2 = new List<string>();
                    indirectDependency2.Add("qm2" + QM_NAME_DELIMITER + channel.channelName);
                    AddDependency(indirectDependencies, "QM1", channel.channelName, indirectDependency2);
                }
            }

        }


        foreach (MQ.Application application in applications)
        {
            List<string> connectedQueues = application.GetConnectedQueues();

            // Becuase we will be modifying the names of connectedQueues to global names, we add indirect dependencies first

            for (int i = 0; i < connectedQueues.Count; i++)
            {
                // Indirect dependency for the connected queues of the application
                List<string> indirectDependencyQueue = new List<string>();
                indirectDependencyQueue.Add(qmgr + QM_NAME_DELIMITER + application.conn);
                AddDependency(indirectDependencies, qmgr, connectedQueues[i], indirectDependencyQueue);
            }

            // Indirect dependency for the application channel
            List<string> indirectDependencyChannel = new List<string>();
            indirectDependencyChannel.Add(qmgr + QM_NAME_DELIMITER + application.conn);
            AddDependency(indirectDependencies, qmgr, application.channel, indirectDependencyChannel);

            // Direct dependency for the application (here connectedQueues are the direct dependency)
            for (int i = 0; i < connectedQueues.Count; i++)
            {
                connectedQueues[i] = qmgr + QM_NAME_DELIMITER + connectedQueues[i];
            }

            connectedQueues.Add(qmgr + QM_NAME_DELIMITER + application.channel); // To save space, append the applciaiotn channel to the end of connectedQueues.
            AddDependency(directDependencies, qmgr, application.conn, connectedQueues);
        }

    }

    private void AddDependency (Dictionary<string, List<string>> dependencies, string qmName, string entityName, List<string> dependency)
    {
        
        if (!dependencies.ContainsKey(qmName + "." + entityName))
        {
            dependencies.Add(qmName + "." + entityName, dependency);
        }
        else
        {
            List<string> oldDependency = new List<string>(dependencies[qmName + "." + entityName]);
            oldDependency.AddRange(dependency);
            dependencies[qmName + "." + entityName] = oldDependency; //Append the new dependency at the end
        }
    }
}
