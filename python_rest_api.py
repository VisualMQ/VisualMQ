import requests
import json
from requests.auth import HTTPBasicAuth


HOST = "localhost"
PORT = "9443"
BASE = "https://{}:{}/ibmmq/rest/v1/admin".format(HOST, PORT)

def getRequest(endpoint):
    # We do not verify SSL certificate because on localhost they are self-signed
    url = BASE + endpoint
    print("Sending request to " + url)
    response = requests.get(url, verify=False, auth=HTTPBasicAuth('admin', 'passw0rd'))
    return response.json()

def getQueueManagers():
    data = getRequest("/qmgr")
    return data.get("qmgr")

# See https://www.ibm.com/docs/en/ibm-mq/9.2?topic=adminqmgr-get
def getQueueManager(qmgr):
    data = getRequest("/qmgr/{}?attributes=*".format(qmgr))
    return data.get("qmgr")

# See https://www.ibm.com/docs/en/ibm-mq/9.0?topic=resources-adminqmgrqmgrnamequeue
def getQueues(qmgr):
    data = getRequest("/qmgr/{}/queue?type=all&attributes=*".format(qmgr))
    return data.get("queue")

def getQueue(qmgr, queue):
    data = getRequest("/qmgr/{}/queue/{}?type=all&attributes=*".format(qmgr, queue))
    return data.get("queue")

# See https://www.ibm.com/docs/en/ibm-mq/9.0?topic=adminqmgrqmgrnamechannel-get#q130500___examples
def getChannels(qmgr):
    data = getRequest("/qmgr/{}/channel?attributes=*".format(qmgr))
    return data.get("channel")



# print(getQueue("QM1", "DEV.QUEUE.1"))
print(json.dumps(getQueueManager("QM1")[0]))
