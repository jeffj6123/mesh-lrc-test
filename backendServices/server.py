from flask import Flask, request, jsonify
import os
import requests

app = Flask(__name__)

BACKEND_ENDPOINT = os.getenv('sendendpoint', 'http://bing.com:80')
REPLICA_COUNT = int(os.getenv('backendreplicacount', 0))
REPLICA_DELIMITER = os.getenv('replicadelimiter', '-')


def hit_all(endpoint='', data=''):
    results = []
    split_backend_point = BACKEND_ENDPOINT.split(':')

    prefix = split_backend_point[0]
    url = split_backend_point[1]
    port = split_backend_point[2]

    for replica in range(REPLICA_COUNT):
        # looks like http://myBackendService-2:3031/fabricid
        request_url = '{prefix}:{url}{replica_delimiter}{replica}:{port}{endpoint}'.format(
            prefix=prefix, url=url, replica_delimiter=REPLICA_DELIMITER, replica=replica,
            port=port, endpoint=endpoint)

        print('hitting {0}'.format(request_url))

        response = None
        status_code = None
        reachable = True
        try:
            r = requests.post(request_url, data=data)
            response = r.content.decode()
            status_code = r.status_code

        except Exception as e:
            reachable = False

        request_info = {
            'url': request_url,
            'response': response,
            'statusCode': status_code,
            'reachable': reachable
        }
        results.append(request_info)
    return results


@app.route("/env", methods=['GET'])
def env_handler():
    env = os.getenv('version', '')
    return env


@app.route("/checkfabricids", methods=['GET'])
def fabric_check_handler():
    results = hit_all('/fabricid')
    return jsonify(results)


@app.route("/all", methods=['GET', 'POST'])
def all_replicas_handler():
    print("received request to send to all")

    response_to_forward = request.data
    results = hit_all(data=response_to_forward)
    return jsonify(results)


@app.route("/many", methods=['GET', 'POST'])
def many_handler():
    print("received request to send many")
    results = []
    quantity = request.args.get('quantity')

    if not quantity:
        quantity = 10

    for _ in range(quantity):
        r = requests.post(BACKEND_ENDPOINT, data=request.data)
        print(r.status_code)
        results.append(r.status_code)
    return jsonify(results)


@app.route("/", methods=['GET', 'POST'])
def main_handler():
    print("received request to forward message")
    r = requests.post(BACKEND_ENDPOINT, data=request.data)
    print(r.status_code)
    return r.content


if __name__ == "__main__":
    port = os.getenv('frontendport', 5000)
    print("receiving requests on {0}".format(port))
    print("forwarding requests to {0}".format(BACKEND_ENDPOINT))
    app.run(host='0.0.0.0', port=port)
