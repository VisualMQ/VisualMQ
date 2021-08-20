/* 
 * The Dependency Graph is constructed by iterating through the MQ entities
 * such as queues, channels, and applications to extract (and sometimes infer)
 * the "connectedness" amongst those entities. 
 * 
 * It is worth noting that the so-called "Dependency Graph" is not actually
 * represented with a graph data strucutre, but rather dictionaries where 
 * the key is the name of the entity and the value is a list of entities 
 * that directly/indirectly relates to the key entity. 
 * 
 * We consider entity A to be a directly dependency of entity B if entity B
 * specifies entity A in some part of the configuration during entity B's
 * initialization. For example, A transmission queue Q1 is a direct dependency of
 * a remote queue Q2 if Q2's specified transmission queue is Q1.
 * 
 * Indirect Dependency is defined to be the opposite of the direct dependency 
 * explained above.
 */
using System.Collections.Generic;

public class DependencyGraph
{
    public Dictionary<string, List<string>> directDependencies;
    public Dictionary<string, List<string>> indirectDependencies;
    public Dictionary<string, List<string>> implicitDependencies; // For future implementation

    private const string QM_NAME_DELIMITER = ".";

    public DependencyGraph()
    {
        directDependencies = new Dictionary<string, List<string>>();
        indirectDependencies = new Dictionary<string, List<string>>();
        // TODO: Implicit Dependencies (i.e., the connectedness betweeen a sender and receiver channnel pair)
        // is beyond the scope of the initial release. Consider for future implementation.
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

    public void ClearDependency()
    {
        directDependencies.Clear();
        indirectDependencies.Clear();
        implicitDependencies.Clear();
    }

    private void AddDependency (Dictionary<string, List<string>> dependencies, string qmName, string entityName, List<string> dependency)
    {
        
        if (!dependencies.ContainsKey(qmName + QM_NAME_DELIMITER + entityName))
        {
            dependencies.Add(qmName + QM_NAME_DELIMITER + entityName, dependency);
        }
        else
        {
            List<string> oldDependency = new List<string>(dependencies[qmName + QM_NAME_DELIMITER + entityName]);
            oldDependency.AddRange(dependency);
            dependencies[qmName + QM_NAME_DELIMITER + entityName] = oldDependency; //Append the new dependency at the end
        }
    }
}
