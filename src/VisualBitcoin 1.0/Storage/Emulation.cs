namespace Storage
{
	class Emulation
	{
		public static void populateStorage()
		{
			var storage = new WindowsAzureStorage();
			var tableClient = storage.RetrieveTableClient();
			var tableReference = tableClient.GetTableReference(WindowsAzureStorage.Table);
			tableReference.Create();
			throw new System.NotImplementedException();
		}
	}
}
