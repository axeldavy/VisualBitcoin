using System;
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Storage
{
	public class Queue
	{
		// Properties.
		public static CloudQueueClient CloudQueueClient { get; private set; }
		public static CloudQueue CloudQueue { get; private set; }


		// Configure and start the queue storage, only one call make on application start.
		public static void Start(string queueName)
		{
			Trace.WriteLine("Configure and start the queue storage");

			CloudQueueClient = WindowsAzure.StorageAccount.CreateCloudQueueClient();
			CloudQueue = CloudQueueClient.GetQueueReference(queueName);
			CloudQueue.CreateIfNotExists();
		}
		
		// Add a message in the queue with a 7 days time span. It could be a good thing to 
		// declare all the (data) models we need in the dedicated folder "Models".
		public static void AddMessage<T>(T model)
		{
			Trace.WriteLine("Add message to queue");

			var message = Serialization.ToXml(model);
			var content = Coding.Code(message);
			var cloudQueueMessage = new CloudQueueMessage(content);
			const int days = 7;
			const int hours = 0;
			const int minutes = 0;
			const int seconds = 0;
			var timeSpan = new TimeSpan(days, hours, minutes, seconds);

			CloudQueue.AddMessage(cloudQueueMessage, timeSpan);
		}

		// Get a message from the queue.
		public static T GetMessage<T>() where T : class
		{
			Trace.WriteLine("Get message from queue.");

			var cloudQueueMessage = CloudQueue.GetMessage();
			var content = cloudQueueMessage.AsString;
			var message = Coding.Decode(content);
			var model = Serialization.FromXml<T>(message);

			return model;
		}
	}
}
