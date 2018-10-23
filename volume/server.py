from flask import Flask, request
import os

STATE_FOLDER_NAME = os.getenv('STATE_FOLDER_NAME', '')


class FileStoreUtility:

    StoreRootFolderEnvVar = ''

    @staticmethod
    def get_state_folder_path(app_name):
        store_root_path = os.getenv(FileStoreUtility.StoreRootFolderEnvVar)

        if not store_root_path:
            store_root_path = 'test'

        if app_name:
            store_root_path = os.path.join(store_root_path, app_name)

        return store_root_path

    @staticmethod
    def write_to_file(file_location, data):
        with open(file_location, 'wb') as fw:
            fw.write(data)

    @staticmethod
    def read_from_file(file_location):
        with open(file_location, 'rb') as fr:
            return fr.read()


app = Flask(__name__)


@app.route("/", methods=['GET', 'POST'])
@app.route("/<file_name>", methods=['GET', 'POST'])
def main_handler(file_name=None):
    if not file_name:
        file_name = 'volume_test_file.txt'

    if request.method == 'POST':
        body = request.data
        print("received request to write to volume share")
        try:
            FileStoreUtility.write_to_file(file_name, body)
        except Exception as e:
            return str(e)

        return '', 200
    elif request.method == 'GET':
        print("received request to read from volume share")
        if os.path.exists(file_name):
            try:
                return FileStoreUtility.read_from_file(file_name)
            except Exception as e:
                print(e)
                return '', 500
        else:
            return 'file does not exist', 404


if __name__ == "__main__":
    port = os.getenv('port', 3030)
    app.run(host='0.0.0.0', port=port)
