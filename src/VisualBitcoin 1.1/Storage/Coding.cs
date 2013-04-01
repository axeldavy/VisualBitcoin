using System.Diagnostics;

namespace Storage
{
	public class Coding
	{
		// To add the zipper see System.IO.Compression.

		public static string Code(string content)
		{
			Trace.WriteLine("Code message");

			return content;
		}

		public static string Decode(string content)
		{
			Trace.WriteLine("Decode message");

			return content;
		}
	}
}
