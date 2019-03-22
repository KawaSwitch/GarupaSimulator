using System.IO;

namespace GarupaSimulator.Util
{
    /// <summary>
    /// <see cref="Directory"/>クラスに関するユーティリティクラス
    /// </summary>
    public static class DirectoryUtil
    {
        /// <summary>
        /// 指定したパスにディレクトリが存在しない場合, すべてのディレクトリとサブディレクトリを作成する
        /// </summary>
        public static DirectoryInfo CreateDirectoryIfNeeded(string path)
        {
            if (Directory.Exists(path))
                return null;

            return Directory.CreateDirectory(path);
        }
    }
}
