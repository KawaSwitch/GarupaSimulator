using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Resources;

namespace GarupaSimulator.Util
{
    /// <summary>
    /// ローカライズ用記述属性
    /// </summary>
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        ResourceManager _resourceManager;
        string _resourceKey;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="resourceKey">リソース名</param>
        /// <param name="resourceType">リソースを含むクラス型</param>
        public LocalizedDescriptionAttribute(string resourceKey, Type resourceType)
        {
            _resourceManager = new ResourceManager(resourceType);
            _resourceKey = resourceKey;
        }

        /// <summary>
        /// Description属性設定値
        /// </summary>
        public override string Description
        {
            get
            {
                string description = _resourceManager.GetString(_resourceKey);
                return string.IsNullOrWhiteSpace(description)
                    ? string.Format("[[{0}]]", _resourceKey)
                    : description;
            }
        }
    }
}
