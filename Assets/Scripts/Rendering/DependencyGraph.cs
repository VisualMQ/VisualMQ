using System;
using System.Collections.Generic;
using UnityEngine;

public class DependencyGraph
{
    public Dictionary<string, List<string>> graph; //TODO: change to private with getter method

    public DependencyGraph()
    {
        graph = new Dictionary<string, List<string>>();
    }

    public void CreateDependencyGraph (List<MQ.Queue> queues, string qmgr) //TODO:Add channels and remote-local queue
    {
        foreach (MQ.Queue queue in queues)
        {
            if (queue is MQ.RemoteQueue)
            {
                List<string> remoteDependency = new List<string>();
                remoteDependency.Add(qmgr + "." + queue.queueName); //Remote queue
                remoteDependency.Add(qmgr + "." + ((MQ.RemoteQueue)queue).transmissionQueueName); //Transmission queue
                remoteDependency.Add(((MQ.RemoteQueue)queue).targetQmgrName + "." + ((MQ.RemoteQueue)queue).targetQueueName); // Target queue

                AddDependency(qmgr, queue.queueName, remoteDependency);// Dependency of the remote queue
                AddDependency(qmgr, ((MQ.RemoteQueue)queue).transmissionQueueName, remoteDependency);// Dependency of the transmission queue
                AddDependency(((MQ.RemoteQueue)queue).targetQmgrName, ((MQ.RemoteQueue)queue).targetQueueName, remoteDependency);// Dependency of the remote target queue

            }

            if (queue is MQ.AliasQueue)
            {
                List<string> aliasDependency = new List<string>();
                aliasDependency.Add(qmgr + "." + queue.queueName); //alias queue
                aliasDependency.Add(qmgr + "." + ((MQ.AliasQueue)queue).targetQueueName);

                graph.Add(qmgr + "." + queue.queueName, aliasDependency); // graph can't contain the alias queue as key before the queue itself is reached
                AddDependency(qmgr, ((MQ.AliasQueue)queue).targetQueueName, aliasDependency);// Dependency of the alias target queue
            }
        }
    }

    private void AddDependency (string qmName, string queueName, List<string> dependency)
    {
        //Debug.Log("current queue name is: " + queueName);
        if (!graph.ContainsKey(qmName + "." + queueName))
        {
            //Debug.Log(graph.Count);
            graph.Add(qmName + "." + queueName, dependency);
            //Debug.Log(graph.Count);
            //Debug.Log("current dependency is: " + string.Join(" , ", dependency.ToArray()));
            //Debug.Log("Dependency added for " + queueName);
        }
        else
        {
            List<string> oldDependency = new List<string>(graph[qmName + "." + queueName]);
            oldDependency.AddRange(dependency);
            graph[qmName + "." + queueName] = oldDependency; //Append the new dependency at the end
        }
    }
}
