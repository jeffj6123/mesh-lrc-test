routes
POST/GET /env
    returns the version env variable(set in template)

POST/GET /checkfabricids
    send a request to /fabricid for each backend service replica(replica count specified in env variables) and returns a list of the their responses and if its reachable

POST/GET /all
    sends 1 request to all the backend service replicas(replica count specified in env variables)  and returns a list of their responses and if reachable

POST/GET /many
    sends 10 requests to the backend service and not any particular replica

POST/GET /
    returns a request forwarded to the backend service and not any particular replica