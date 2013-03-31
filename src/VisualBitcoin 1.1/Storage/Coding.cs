using System.Diagnostics;

namespace Storage
{
	public class Coding
	{
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
