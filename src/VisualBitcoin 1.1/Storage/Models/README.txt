////////////////////////////////////////////////////////////////////////////////
// Storage Models Documentation                                               //
////////////////////////////////////////////////////////////////////////////////

All type-classes contain in this project are:
	- Block
	- BlockReference
	- Transactions
You will found below where is used each of this type-classes.


////////////////////////////////////////////////////////////////////////////////
1. Block

Block is the main data structure. The BitcoinWorkerRole produces the instances
of this class which are directly stored in the primary storage. These
representation of the blockchain's blocks can be different of the original
information associate with blocks.
The Block type-class uses the Transactions type-class.

////////////////////////////////////////////////////////////////////////////////
2. BlockReference

BlockReference are stored in the queue. It allows the BitnetWorkerRole to send  
news (which new blocks have been send) to the StatWorkerRole.

////////////////////////////////////////////////////////////////////////////////
3. Transactions

Transactions is a subpart for the Block type-class.

	TODO
	- Is "split the storage between a primary storage and a structured storage" 
	  a good idea?
	- Is "use the table storage to have structured data" a good idea?
	- How could we organize the storage to enable interessant queries from the  
	  WebRole?
	- How could we organize the storage to enable fast queries from the        
	  WebRole?

////////////////////////////////////////////////////////////////////////////////
4. BlockClear

Created to separate Transactions data and Block data. Transaction's array is replaced by array of transactions ID.