using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GarupaSimulator.File
{
    /// <summary>
    /// オブジェクトのXMLシリアル化, デシリアル化を行う
    /// </summary>
    public static class XmlSerializer
    {
        /// <summary>
        /// オブジェクトの内容をファイルから読み込み復元する
        /// </summary>
        /// <param name="objType">保存するオブジェクトの型</param>
        /// <param name="path">読み込むファイル名</param>
        /// <returns>復元されたオブジェクト</returns>
        public static object LoadFromBinaryFile(Type objType, string path)
        {
            if (!System.IO.File.Exists(path))
                return null;

            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(objType);

            using (var sr = new System.IO.StreamReader(path, new System.Text.UTF8Encoding(false)))
            {
                try
                {
                    // 読み込んでデシリアル化
                    var obj = Convert.ChangeType(serializer.Deserialize(sr), objType);
                    return obj;
                }
                catch
                {
                    // とりあえずnull返す
                    return null;
                }
            }
        }

        /// <summary>
        /// オブジェクトの内容をファイルに保存する
        /// </summary>
        /// <param name="obj">保存するオブジェクト</param>
        /// <param name="objType">保存するオブジェクトの型</param>
        /// <param name="path">保存先のファイル名</param>
        public static void SaveToBinaryFile(object obj, Type objType, string path)
        {
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(objType);

            using (var sw = new System.IO.StreamWriter(path, false, new System.Text.UTF8Encoding(false)))
            {
                // シリアル化し, XMLファイルに保存する
                serializer.Serialize(sw, obj);
            }
        }
    }

    /// <summary>
    /// オブジェクトのバイナリシリアル化, デシリアル化を行う
    /// </summary>
    public static class BinarySerializer
    {
        /// <summary>
        /// オブジェクトの内容をファイルから読み込み復元する
        /// </summary>
        /// <param name="path">読み込むファイル名</param>
        /// <returns>復元されたオブジェクト</returns>
        public static object LoadFromBinaryFile(string path)
        {
            if (!System.IO.File.Exists(path))
                return null;

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                try
                {
                    // 読み込んでデシリアル化
                    var obj = formatter.Deserialize(fs);
                    return obj;
                }
                catch
                {
                    // とりあえずnull返す
                    return null;
                }
            }
        }

        /// <summary>
        /// オブジェクトの内容をファイルに保存する
        /// </summary>
        /// <param name="obj">保存するオブジェクト</param>
        /// <param name="path">保存先のファイル名</param>
        public static void SaveToBinaryFile(object obj, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter bf = new BinaryFormatter();

                // シリアル化して書き込む
                bf.Serialize(fs, obj);
            }
        }
    }
}
