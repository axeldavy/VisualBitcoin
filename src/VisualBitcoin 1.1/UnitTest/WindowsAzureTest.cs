using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;

namespace UnitTest
{
	[TestClass]
	public class WindowsAzureTest
	{
		public const string ContainerName = "visualbitcoincontainerunittest";
		public const string TableName = "visualbitcointableunittest";
		public const string QueueName = "visualbitcoinqueueunittest";

		// Test the configuration and the start of the storage.
		[TestMethod]
		public void StartStorage()
		{
			// Arrange.
			const bool useDevelopmentStorage = true;
			const string connectionString = "";

			// Act.
			WindowsAzure.Start(useDevelopmentStorage, connectionString, ContainerName, TableName, QueueName);			
		}
	}
}
