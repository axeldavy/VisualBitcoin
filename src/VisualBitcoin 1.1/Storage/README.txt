////////////////////////////////////////////////////////////////////////////////
// Storage Project Documentation                                              //
////////////////////////////////////////////////////////////////////////////////

All classes contain in this project are:
	1. Blob
	2. Coding
	3. Queue
	4. Serialization
	5. Storage
	6. Table
	7. WindowsAzure
You will found below a short description of the services provide by each of     
these classes.


////////////////////////////////////////////////////////////////////////////////
1. Blob Class

Manage the blob storage. You will find methods to start the service, add blobs, 
retrieve blobs, remove blobs. To use this class consider the following methods

	- void UploadBlockBlob<TModel>(string blockBlobName, TModel model)
		You do not have to care about the container where the model is upload,
		consider that you just throw a blob on the cloud by passing the model
		type, the blob name and the model itself.

	- TModel DownloadBlockBlob<TModel>(string blockBlobName)
		You don't have to care about the container too.


////////////////////////////////////////////////////////////////////////////////
2. Coding Class

Manage the efficency of connections with the storage with a code and decode     
method.

	TODO
	- Add a zipper

////////////////////////////////////////////////////////////////////////////////
3. Queue Class

Manage the queue storage. You will find methods to start the service, add       
messages, get messages. You have to use this class if you want to add some data 
in the storage. To use this class consider the following methods

	- void PushMessage<TModel>(TModel model)
	- TModel PopMessage<TModel>()


	TODO
	- TModel is always set to BlockReference, perhaps it will be a good idea to
	  simplify these methods.

////////////////////////////////////////////////////////////////////////////////
4. Serialization Class

Manage the serialization and try to hide the conversion between the instance of 
a class and its XML string.

	TODO
	- Analyze the current default serialization
	- Replace the current serialization with something better
	- Perhaps change the XML to JSON serialization to use the d3js library

////////////////////////////////////////////////////////////////////////////////
5. Storage Class

Historic class which contains all the source code. This code has to be organized
and dispatched in the other classes.

	TODO
	- Dispatched the whole code in other classes
	- Delete this class

////////////////////////////////////////////////////////////////////////////////
6. Table Class

Manage the table storage. You will find methods to start the service, add rows, 
retrieve rows, remove rows. You have not to use this class and consider it    
only as a tool for the other classes which manage the storage.

	TODO
	- See if we really need a table (delete this class if not)
	- Adding rows
	- Retrieving rows
	- Removing rows

////////////////////////////////////////////////////////////////////////////////
7. WindowsAzure Class

Manage the windows azure storage account. You will find a method to start the   
service, only one call is made for this method at the begining of the
application. You have not to use this class and consider it only as a tool for  
other classes, provides access to the storage.
