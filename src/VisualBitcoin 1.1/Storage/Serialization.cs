using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Storage
{
	public class Serialization
	{
		// Retrieve an object from a XML description of it.
		public static T FromXml<T>(string xmlString) where T : class
		{
			Trace.WriteLine("Deserialize " + typeof (T));

			var xmlSerializer = new XmlSerializer(typeof (T));
			var stringReader = new StringReader(xmlString);
			var xmlTextReader = new XmlTextReader(stringReader);
			var xmlObject = xmlSerializer.Deserialize(xmlTextReader) as T;

			xmlTextReader.Close();
			stringReader.Close();

			return xmlObject;
		}

		// Create the XML description of an object.
		public static string ToXml<T>(T xmlObject)
		{
			Trace.WriteLine("Serialize " + typeof (T));

			var xmlSerializer = new XmlSerializer(typeof (T));
			var stringWriter = new StringWriter();
			var xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlSerializer.Serialize(xmlTextWriter, xmlObject);
			var xmlString = stringWriter.ToString();

			xmlTextWriter.Close();
			stringWriter.Close();

			return xmlString;
		}
	}
}
