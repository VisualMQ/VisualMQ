using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MQ;


public class RegularReader : MonoBehaviour
{

    State internalState;


/* To be changed to keep track of QMs*/
    private Dictionary<string, bool> renderedQMs = new Dictionary<string, bool>();
    private Dictionary<string, bool> renderedQueues = new Dictionary<string, bool>();


    void Start()
    {
        //Step one: create an Http client (QueueManager class), remember to set the username and apikey
        string testUsername = "NA";
        string testAPIKey = "NA";
        string testQMUrl = "NA";
        string testQmgr = "QM1";
        QueueManager mq = new QueueManager(testQMUrl, testQmgr, testUsername, testAPIKey);

        //Step two: create QMInfo, QueuesInfo, and List of MessagesInfo objects
        string QMInfo = mq.GetQmgr();
        QMInfo QM1 = new QMInfo(QMInfo);

        string allqueue = mq.GetAllQueues();
        QueuesInfo Qs1 = new QueuesInfo(allqueue);

        List<MessagesInfo> allQMessages = new List<MessagesInfo>();
        

        //Step three: create a state object that consists of the three objects/list of objects in step three
        internalState = new State(QM1, Qs1, allQMessages);

        // Display this current QM1

        CreateVisual.VisualizeQM(QM1);

        foreach(QueueStorage storage in internalState.QueuesInfo.storage)
        {
            CreateVisual.VisualizeQueue(QM1, storage);
        }



        InvokeRepeating("ContinousRender", 1f, 10f);  //1s delay, repeat every 1s

    }

    // Update is called once per frame
    void ContinousRender()
    {
        /*Have to iterate over this here when the proper internal state has been generated*/
        QMInfo qm1 = internalState.QMInfo;

        /* Two things we have to do within the loop body: FIRST we need to render any new objects. */
        if (qm1.obj == null && renderedQMs.ContainsKey(qm1.name)){
            /* IF the object is null it has NOT been rendered. HOWEVER its also in the internal state
             therefore we must render this new QM.
             assumption: QM names are unique.
             */

            CreateVisual.VisualizeQM(qm1);
            renderedQMs.Add(qm1.name, true);
        }

        for(int i = 0; i < internalState.QMInfo.name.Length; i++){
           string currName = internalState.QueuesInfo.name[i];
           QueueStorage currStorage = internalState.QueuesInfo.storage[i];

            /*
             * Same checks as above. See if this is not NULL and within the dict
             */

            if (currStorage.obj == null && renderedQueues.ContainsKey(currName))
            {
                /* IF the object is null it has NOT been rendered. HOWEVER its also in the internal state
                 therefore we must render this new QM.
                 assumption: QM names are unique.
                 */

                CreateVisual.VisualizeQueue(qm1,currStorage);
                renderedQueues.Add(currName, true);
            }
        }

        /** Helpers for testing TODO: We need a proper update function **/

    }
}
