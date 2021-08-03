using System;
using System.Collections.Generic;

namespace MQ
{
    public class Application
    {

        public class ConnectedObject
        {
            public string objname;
            // There can be multiple types: QUEUE, TOPIC, QMGR
            // Look for QUEUE
            public string objtype;
            public string hstate;
            public string reada;
            public List<string> openopts;
        }

        public string conn;
        public string channel;
        public string type;
        public string conntag;
        public string conname;
        public List<string> connopts;
        public string appltype;
        public string appldesc;
        public string appltag;
        public List<ConnectedObject> connectedObjects;

        public List<string> GetConnectedQueues()
        {
            List<string> connectedQueues = new List<string>();
            foreach (ConnectedObject conn in connectedObjects)
            {
                if (conn.objtype == "QUEUE")
                {
                    connectedQueues.Add(conn.objname);
                }
            }
            return connectedQueues;
        }
    }
}