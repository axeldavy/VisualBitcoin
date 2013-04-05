using System.Diagnostics;

namespace Storage
{
	public class Coding
	{
		// To add the zipper see System.IO.Compression.

		public static string Code(string content)
		{
			Trace.WriteLine("Message coded",
				"VisualBitcoin.Storage.Coding Information");

			return content;
		}

		public static string Decode(string content)
		{
			Trace.WriteLine("Message decoded",
				"VisualBitcoin.Storage.Coding Information");

			return content;
		}
	}
}
