////////////////////////////////////////////////////////////////////////////////
// Storage Project Documentation                                              //
////////////////////////////////////////////////////////////////////////////////

All classes contain in this project are:
	- Blob
	- Block
	- Coding
	- Queue
	- Storage
	- Table
	- WindowsAzure
You will found below a short description of the services provide by each of     
these classes.


////////////////////////////////////////////////////////////////////////////////
1. Blob Class

Manage the blob storage. You will find methods to start the service, add blobs, 
retrieve blobs, remove blobs. You have not to use this class and consider it    
only as a tool for the other classes which manage the storage.

	TODO
	- Adding blobs.
	- Retrieving blobs.
	- Removing blobs.

////////////////////////////////////////////////////////////////////////////////
2.Block Class

Give the schema of the blocks contain in our storage. A different model is      
defined in the WebRole which contains only the relevant information. These      
representation of the blockchain's blocks can be different of the original      
information associate with blocks.

	TODO
	- Find the data structures for transactions.

////////////////////////////////////////////////////////////////////////////////
3. Coding Class

Manage the efficency of connections with the storage with a code and decode     
method.

	TODO
	- Add a zipper.

////////////////////////////////////////////////////////////////////////////////
4. Queue Class

Manage the queue storage. You will find methods to start the service, add       
messages, get messages. You have to use this class if you want to add some data 
in the storage.

	TODO
	- ...

////////////////////////////////////////////////////////////////////////////////
5. Storage Class

Historic class which contains all the source code. This code has to be organized
and dispatched in the other classes.

	TODO
	- Dispatched the whole code in other classes.

////////////////////////////////////////////////////////////////////////////////
6. Table Class

Manage the table storage. You will find methods to start the service, add rows, 
retrieve rows, remove rows. You have not to use this class and consider it    
only as a tool for the other classes which manage the storage.

	TODO
	- Adding rows.
	- Retrieving rows.
	- Removing rows.

////////////////////////////////////////////////////////////////////////////////
7. WindowsAzure Class

Manage the windows azure storage account. You will find a method to start the   
service, only one call is made for this method at the begining of the
application. You have not to use this class and consider it only as a tool for  
other classes, provides access to the storage.

	TODO
	- ...
