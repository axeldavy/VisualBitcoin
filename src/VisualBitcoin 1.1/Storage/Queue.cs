using System;
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure;
using System.Threading;

namespace Storage
{
	public class Queue
	{
		// Properties.
        private CloudQueueClient cloudQueueClient;
        private CloudQueue cloudQueue;

        private const string queueName = "visualbitcoinqueue";

		// Configure and start the queue storage, only one call make on application start.
		public Queue(CloudQueueClient cloudQueueClient)
		{
			Trace.WriteLine("Start", "VisualBitcoin.Storage.Queue Information");

            this.cloudQueueClient = cloudQueueClient;
			cloudQueue = cloudQueueClient.GetQueueReference(queueName);
			cloudQueue.CreateIfNotExists();
		}

		// TODO: Figure out the point of this method 
        // Pop all messages in the queue.
		/*public static void Reset(bool resetBlobBlocksEnable)
		{
			Trace.WriteLine("Reset", "VisualBitcoin.Storage.Queue Information");

			CloudQueue.Clear();

			if (resetBlobBlocksEnable)
				return;

			var blockList = Blob.GetBlockList();
			foreach (var blockName in blockList)
			{
				var block = Blob.GetBlock(blockName);
				var blockReference = new Models.BlockReference(block.Hash);
				PushMessage(blockReference);
			}
		}*/

		// Push a message in the queue with a 7 days time span. It could be a good thing to
		// declare all the (data) models we need in the dedicated folder "Models". All our 
		// models in one place.
		public void PushMessage<TModel>(TModel model)
		{
			Trace.WriteLine("Message pushed", "VisualBitcoin.Storage.Queue Information");

			var message = Serialization.ToXml(model);
			var content = Coding.Code(message);
			var cloudQueueMessage = new CloudQueueMessage(content);
			const int days = 7;
			const int hours = 0;
			const int minutes = 0;
			const int seconds = 0;
			var timeSpan = new TimeSpan(days, hours, minutes, seconds);

			cloudQueue.AddMessage(cloudQueueMessage, timeSpan);
		}

		// Pop a message from the queue.
		public TModel PopMessage<TModel>() where TModel : class
		{
			Trace.WriteLine("Message popped", "VisualBitcoin.Storage.Queue Information");
            try
            {
                var cloudQueueMessage = cloudQueue.GetMessage();
                var content = cloudQueueMessage.AsString;
                var message = Coding.Decode(content);
                var model = Serialization.FromXml<TModel>(message);

                return model;
            }
            catch
            {
                Thread.Sleep(500);
                return PopMessage<TModel>();
            }
			
		}
	}
}
