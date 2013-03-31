using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;

namespace UnitTest
{
	[TestClass]
	public class CodingTest
	{
		// Test if coding is reversible.
		[TestMethod]
		public void DecodeCoded()
		{
			// Arrange.
			const string content = "Content.";

			// Act.
			var codedContent = Coding.Code(content);
			var decodedCodedContent = Coding.Decode(codedContent);

			// Assert.
			Assert.AreEqual(content, decodedCodedContent);
		}
	}
}
