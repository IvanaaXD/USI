
using LangLang.Model;
using LangLang.Storage.Serialization;
using System.Collections.Generic;
using System.IO;

namespace LangLang.Storage
{

    /*
     * The class responsible for saving data in the file system.
     * This class makes sure that the corresponding file exists,
     * writes and reads data from it. Storage uses Serializer
     * a class for converting an object to a string and vice versa.
    */
    public class Storage<T> where T : ISerializable, new()
    {
        private readonly string _fileName = @"../../../../LangLang/Data/{0}";
        private readonly Serializer<T> _serializer = new Serializer<T>();

        public Storage(string fileName)
        {
            _fileName = string.Format(_fileName, fileName);
        }
        public List<T> Load()
        {
            // If the file does not exist, create a new file i
            // close the stream for writing to the file
            if (!File.Exists(_fileName))
            {
                FileStream fs = File.Create(_fileName);
                fs.Close();
            }

            IEnumerable<string> lines = File.ReadLines(_fileName);
            List<T> objects = _serializer.FromCSV(lines);

            return objects;
        }

        public void Save(List<T> objects)
        {
            string serializedObjects = _serializer.ToCSV(objects);
            using (StreamWriter streamWriter = new StreamWriter(_fileName))
            {
                streamWriter.Write(serializedObjects);
            }
        }
    }
}