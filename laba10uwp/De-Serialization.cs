using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace laba10uwp
{
    class De_Serialization
    {
        public static async void Serialization<T>(string filName, CreationCollisionOption creation, object variable)
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            StorageFile file = await folder.CreateFileAsync(filName, creation);

            Stream stream = await file.OpenStreamForWriteAsync();
            xml.Serialize(stream, variable);
            stream.Close();
        }

        public static async Task<T> Deserialisation<T>(string filName, CreationCollisionOption creation, object variable)
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            StorageFile file = await folder.CreateFileAsync(filName, creation);

            string str = await FileIO.ReadTextAsync(file);

            Stream stream = await file.OpenStreamForWriteAsync();
            if (str.Length != 0) variable = (T)xml.Deserialize(stream);
            stream.Close();

            T result = (T)Convert.ChangeType(variable, typeof(T));
            return result;
        }
    }
}
