using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace Sources.cdreyer.SaveSystem
{
    public static class SaveSystem
    {
        static StringBuilder GetFilePath(string fileName) => new StringBuilder($"{Application.persistentDataPath}/{fileName}.data");

        public static void Save<TData>(TData data, string fileName) where TData : class
        {
            using (FileStream fs = new(GetFilePath(fileName).ToString(), FileMode.Create))
            {
                BinaryFormatter bf = new();
                bf.Serialize(fs, data);
            }
        }

        public static TData Load<TData>(string fileName) where TData : class
        {
            try
            {
                using (FileStream fs = new(GetFilePath(fileName).ToString(), FileMode.Open))
                {

                    BinaryFormatter bf = new();
                    TData data = bf.Deserialize(fs) as TData;

                    fs.Close();
                    return data;
                }
            }
            catch
            {
                GameLogger.GameLogger.Log($"data for {typeof(TData).Name} not found", "yellow");

                if (typeof(ISavable<TData>).IsAssignableFrom(typeof(TData)))
                {
                    TData data = (TData)Activator.CreateInstance(typeof(TData));
                    return (data as ISavable<TData>).GetBase();
                }

                return null;
            }
        }

        public static void DeleteData(string fileName)
        {
            File.Delete(GetFilePath(fileName).ToString());
        }
    }
}