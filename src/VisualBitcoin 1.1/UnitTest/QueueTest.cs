using Microsoft.VisualStudio.TestTools.UnitTesting;
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
			Queue.PushMessage(message);
			var transmittedMessage = Queue.PopMessage<string>();
			
			Assert.AreEqual(message, transmittedMessage);
		}
	}
}
