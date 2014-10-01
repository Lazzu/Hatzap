using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Hatzap.Utilities
{
    /// <summary>
    /// Quick xml reader class.
    /// </summary>
    public class XMLReader
    {
        /// <summary>
        /// Reads a file to an object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="file">File.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T FromFile<T>(string file)
        {
            using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                return FromStream<T>(stream);
            }
        }

        /// <summary>
        /// Reads a XML byte array to object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="xml">Xml.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T FromXML<T>(byte[] xml)
        {
            using (MemoryStream stream = new MemoryStream(xml))
            {
                return FromStream<T>(stream);
            }
        }

        /// <summary>
        /// Reads a XML string to object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="xml">Xml.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T FromXML<T>(string xml, System.Text.Encoding encoder)
        {
            using (MemoryStream stream = new MemoryStream(encoder.GetBytes(xml)))
            {
                return FromStream<T>(stream);
            }
        }

        /// <summary>
        /// Reads the stream containing xml to object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="stream">Stream.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T FromStream<T>(Stream stream)
        {
            XmlSerializer r = new XmlSerializer(typeof(T));
            return (T)r.Deserialize(stream);
        }
    }
}
