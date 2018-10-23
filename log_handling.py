# # import logging
# # import time
# #
# #
# # class LogDBHandler(logging.Handler):
# #     '''
# #     Customized logging handler that puts logs to the database.
# #     pymssql required
# #     '''
# #     def __init__(self):
# #         logging.Handler.__init__(self)
# #
# #     def emit(self, record):
# #         # Set current time
# #         tm = time.strftime("%Y-%m-%d %H:%M:%S", time.localtime(record.created))
# #         # Clear the log message so it can be put to db via sql (escape quotes)
# #         self.log_msg = record.msg
# #         self.log_msg = self.log_msg.strip()
# #         self.log_msg = self.log_msg.replace('\'', '\'\'')
# #
# #
# # logdb = LogDBHandler()
# # logging.getLogger('').addHandler(logdb)
# #
# # logging.info('test')
# #
# # log = logging.getLogger('MY_LOGGER')
# # log.setLevel('INFO')
# # log.info('test2')
# #
# # class LogHandler:
# #
#
# # ----------------------------------------------------------------------------------
# # MIT License
# #
# # Copyright(c) Microsoft Corporation. All rights reserved.
# #
# # Permission is hereby granted, free of charge, to any person obtaining a copy
# # of this software and associated documentation files (the "Software"), to deal
# # in the Software without restriction, including without limitation the rights
# # to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# # copies of the Software, and to permit persons to whom the Software is
# # furnished to do so, subject to the following conditions:
# # ----------------------------------------------------------------------------------
# # The above copyright notice and this permission notice shall be included in all
# # copies or substantial portions of the Software.
# #
# # THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# # IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# # FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# # AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# # LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# # OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
# # SOFTWARE.
#
#
#
# import os, uuid, sys
# from azure.storage.blob import BlockBlobService, PublicAccess
#
# # ---------------------------------------------------------------------------------------------------------
# # Method that creates a test file in the 'Documents' folder.
# # This sample application creates a test file, uploads the test file to the Blob storage,
# # lists the blobs in the container, and downloads the file with a new name.
# # ---------------------------------------------------------------------------------------------------------
# # Documentation References:
# # Associated Article - https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-python
# # What is a Storage Account - http://azure.microsoft.com/en-us/documentation/articles/storage-whatis-account/
# # Getting Started with Blobs-https://docs.microsoft.com/en-us/azure/storage/blobs/storage-python-how-to-use-blob-storage
# # Blob Service Concepts - http://msdn.microsoft.com/en-us/library/dd179376.aspx
# # Blob Service REST API - http://msdn.microsoft.com/en-us/library/dd135733.aspx
# # ----------------------------------------------------------------------------------------------------------
#
#
# def run_sample():
#     try:
#         # Create the BlockBlockService that is used to call the Blob service for the storage account
#         block_blob_service = BlockBlobService(account_name='jejarryblob', account_key='SAcJahi0CvfTivvdduc194Ct7R5wfN3ESMynJeGkcg89hKjCP2ehl+xn+cyCEuxjJ5oTsDZrh0pU8GjpVXEuMg==')
#
#         # Create a container called 'quickstartblobs'.
#         container_name ='quickstartblobs'
#         block_blob_service.create_container(container_name)
#
#         # Set the permission so the blobs are public.
#         block_blob_service.set_container_acl(container_name, public_access=PublicAccess.Container)
#
#         # Create a file in Documents to test the upload and download.
#         local_path=os.path.expanduser("~/Documents")
#         local_file_name ="QuickStart_" + str(uuid.uuid4()) + ".txt"
#         full_path_to_file =os.path.join(local_path, local_file_name)
#
#         # Write text to the file.
#         file = open(full_path_to_file,  'w')
#         file.write("Hello, World!")
#         file.close()
#
#         print("Temp file = " + full_path_to_file)
#         print("\nUploading to Blob storage as blob" + local_file_name)
#
#         # Upload the created file, use local_file_name for the blob name
#         block_blob_service.create_blob_from_path(container_name, local_file_name, full_path_to_file)
#
#         # List the blobs in the container
#         print("\nList blobs in the container")
#         generator = block_blob_service.list_blobs(container_name)
#         for blob in generator:
#             print("\t Blob name: " + blob.name)
#
#         # Download the blob(s).
#         # Add '_DOWNLOADED' as prefix to '.txt' so you can see both files in Documents.
#         full_path_to_file2 = os.path.join(local_path, str.replace(local_file_name ,'.txt', '_DOWNLOADED.txt'))
#         print("\nDownloading blob to " + full_path_to_file2)
#         block_blob_service.get_blob_to_path(container_name, local_file_name, full_path_to_file2)
#
#         sys.stdout.write("Sample finished running. When you hit <any key>, the sample will be deleted and the sample "
#                          "application will exit.")
#         sys.stdout.flush()
#         input()
#
#         # Clean up resources. This includes the container and the temp files
#         block_blob_service.delete_container(container_name)
#         os.remove(full_path_to_file)
#         os.remove(full_path_to_file2)
#     except Exception as e:
#         print(e)
#
#
# # Main method.
# if __name__ == '__main__':
#     run_sample()
#
#
#


import json

s = """{\r\n  "error": {\r\n    "code": "ReferencedResourceCannotBeDeleted",\r\n    "message": "Resource /subscriptions/13ad2c84-84fa-4798-ad71-e70c07af873f/resourcegroups/JEJARRYTEST/providers/Microsoft.ServiceFabricMesh/networks/HELLOWORLDNETWORK is referenced by 1 resources and cannot be deleted.",\r\n    "details": [],\r\n    "innerEr
ror": "Microsoft.ServiceFabric.Rp.Errors.WrpException: Resource /subscriptions/13ad2c84-84fa-4798-ad71-e70c07af873f/resourcegroups/JEJARRYTEST/providers/Microsoft.ServiceFabricMesh/networks/HELLOWORLDNETWORK is referenced by 1 resources and cannot be deleted.\\r\\n   at Microsoft.SeaBreeze.ResourceStateMachine.ResourceStateMachineBase`5.<DeleteAsync>d__7
.MoveNext() in C:\\\\src\\\\SF-SeaBreeze\\\\src\\\\ResourceStateMachine\\\\ResourceStateMachineBase.cs:line 109\\r\\n--- End of stack trace from previous location where exception was thrown ---\\r\\n   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()\\r\\n   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotifi
cation(Task task)\\r\\n   at System.Runtime.CompilerServices.TaskAwaiter`1.GetResult()\\r\\n   at Microsoft.SeaBreeze.Actors.RealActorProxy`2.<InvokeMethod>d__16`1.MoveNext() in C:\\\\src\\\\SF-SeaBreeze\\\\src\\\\Actors\\\\RealActorProxy.cs:line 161\\r\\n--- End of stack trace from previous location where exception was thrown ---\\r\\n   at System.Runti
me.ExceptionServices.ExceptionDispatchInfo.Throw()\\r\\n   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)\\r\\n   at System.Runtime.CompilerServices.TaskAwaiter`1.GetResult()\\r\\n   at Microsoft.SeaBreeze.Actors.RealActorProxy`2.<InvokeMethodWithReturnValue>d__15`1.MoveNext() in C:\\\\src\\\\SF-SeaBreez
e\\\\src\\\\Actors\\\\RealActorProxy.cs:line 149\\r\\n--- End of stack trace from previous location where exception was thrown ---\\r\\n   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()\\r\\n   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)\\r\\n   at System.Runtime.CompilerServices.Ta
skAwaiter`1.GetResult()\\r\\n   at Microsoft.SeaBreeze.NetworkResourceManager.Operation.Networks.DeleteNetworkOperation.<ExecuteAsync>d__6.MoveNext() in C:\\\\src\\\\SF-SeaBreeze\\\\src\\\\NetworkResourceManager\\\\Operation\\\\Networks\\\\DeleteNetworkOperation.cs:line 66\\r\\n--- End of stack trace from previous location where exception was thrown ---\
\r\\n   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()\\r\\n   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)\\r\\n   at Microsoft.ServiceFabric.Rp.Operation.OperationBaseWithoutInstrumentation`1.<RunAsync>d__49.MoveNext()"\r\n  }\r\n}
"""
s.replace("\r\n", '')
print(s)