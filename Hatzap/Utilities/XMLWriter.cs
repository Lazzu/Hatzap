using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Hatzap.Utilities
{
    public class XMLWriter
    {
        /// <summary>
        /// Write object to xml string
        /// </summary>
        /// <param name="obj">Object.</param>
        public string ToString(object obj)
        {
            using (Stream s = ToStream(obj))
            {
                using (StreamReader r = new StreamReader(s))
                {
                    return r.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Write object to xml file
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="file">File.</param>
        public void ToFile(object obj, string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
                ToStream(obj, fs);
            }
        }

        /// <summary>
        /// Write object to xml stream
        /// </summary>
        /// <param name="obj">Object.</param>
        public Stream ToStream(object obj)
        {
            MemoryStream stream = new MemoryStream();

            ToStream(obj, stream);

            return stream;
        }

        /// <summary>
        /// Write object to xml stream
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="stream">Stream.</param>
        public void ToStream(object obj, Stream stream)
        {
            XmlSerializer s = new XmlSerializer(obj.GetType());
            using (StreamWriter w = new StreamWriter(stream))
            {
                s.Serialize(w, obj);
            }
        }
    }
}
