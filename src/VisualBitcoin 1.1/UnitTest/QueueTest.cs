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
			const string connectionString = "";
			const string message = "Message.";

			//WindowsAzure.Start(useDevelopmentStorage, connectionString, containerName, tableName, queueName);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            Queue queue = new Queue(storageAccount.CreateCloudQueueClient());
            queue.PushMessage(message);
			var transmittedMessage = queue.PopMessage<string>();
			
			Assert.AreEqual(message, transmittedMessage);
		}
	}
}
