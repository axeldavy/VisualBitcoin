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

			WindowsAzure.Start(useDevelopmentStorage, connectionString, containerName, tableName, queueName);
			Queue.AddMessage(message);
			var transmittedMessage = Queue.GetMessage();
			
			Assert.AreEqual(message, transmittedMessage);
		}
	}
}
