from flask import Flask, request, jsonify
import os

app = Flask(__name__)

FABRIC_ID = 'fabric_id'
ECHO_TEXT = " + Back-end service"


@app.route("/fabricid", methods=['GET', 'POST'])
def env_handler():
    print("sending fabric id")
    response = os.getenv(FABRIC_ID, '')
    return response


@app.route("/", methods=['GET', 'POST'])
def main_handler():
    print("echoing message")
    response = request.data.decode() + ECHO_TEXT
    return response


if __name__ == "__main__":
    port = os.getenv('backendport', 3031)
    print("receiving requests on {0}".format(port))
    app.run(host='0.0.0.0', port=port)