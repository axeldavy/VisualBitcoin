using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;

namespace UnitTest
{
	[TestClass]
	public class QueueTest
	{
		private const string Message = "Message.";

		[TestMethod]
		public void AddMessageToQueue()
		{
			// Act.
			Queue.AddMessage(Message);
		}

		[TestMethod]
		public void GetMessageFromQueue()
		{
			// Arrange.
			AddMessageToQueue();

			// Act.
			var message = Queue.GetMessage();

			// Assert.
			Assert.AreNotEqual(null, message);
		}
	}
}
