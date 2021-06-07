import requests
from requests.auth import HTTPBasicAuth


HOST = "localhost"
PORT = "9443"
BASE = "https://{}:{}/ibmmq/rest/v2/admin".format(HOST, PORT)

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

# For some reason not working, we might need cloud instance see https://stackoverflow.com/questions/57179975/in-ibm-mq-java-how-do-i-access-all-the-queues-under-a-given-queue-manager-pref
def getQueues(qmgr):
    data = getRequest("/qmgr/{}/queue?type=all&attributes=*".format(qmgr))
    print(data)
    return data.get("queue")

def getQueue(queue):
    data = getRequest("/queue/{}?attributes=*".format(queue))
    return data.get("queue")

print(getQueueManagers())
print(getQueueManager("QM1"))
print(getQueues("QM1"))
# print(getQueueManager("QM1"))

