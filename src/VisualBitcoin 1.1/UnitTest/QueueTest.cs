using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Storage;

namespace UnitTest
{
	[TestClass]
	public class QueueTest
	{
		// Test the configuration and the start of the storage.
		[TestMethod]
		public void MessageThroughQueue()
		{
			const bool useDevelopmentStorage = true;
			const string connectionString = "";
			const string containerName = "visualbitcoincontainerunittest";
			const string tableName = "visualbitcointableunittest";
			const string queueName = "visualbitcoinqueueunittest";
			const string message = "Message.";

			//WindowsAzure.Start(useDevelopmentStorage, connectionString, containerName, tableName, queueName);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            Queue queue = new Queue(storageAccount.CreateCloudQueueClient(), queueName);
            queue.PushMessage(message);
			var transmittedMessage = queue.PopMessage<string>();
			
			Assert.AreEqual(message, transmittedMessage);
		}
	}
}
