using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GarupaSimulator.File
{
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
