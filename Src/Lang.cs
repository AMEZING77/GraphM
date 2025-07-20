using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Src;
public static class Lang
{
    private const string zh_CN = "zh-CN";

    private const string en_US = "en-US";

    /// <summary>
    /// 内部使用的支持语言列表。
    /// </summary>
    private static readonly HashSet<string> SupportedLangs = new HashSet<string>()
    {
        zh_CN,
        en_US
    };

    /// <summary>
    /// 将输入的语言代码标准化为支持的格式。
    /// </summary>
    /// <param name="languageCode"></param>
    /// <returns></returns>
    public static string NormalizeLanguageCode(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
            return "zh-CN";

        // 处理简写形式
        return languageCode.ToLowerInvariant() switch
        {
            "zh" or "zh-cn" => zh_CN,
            "en" or "en-us" => en_US,
            _ => zh_CN
        };
    }

    /// <summary>
    /// 返回所有支持的语言代码只读列表。
    /// </summary>
    /// <returns></returns>
    public static IReadOnlyCollection<string> GetAllSupportedLanguages() =>
        SupportedLangs.ToList().AsReadOnly();

    /// <summary>
    /// 判断给定的语言代码是否为支持的语言。
    /// </summary>
    /// <param name="languageCode"></param>
    /// <returns></returns>
    public static bool IsSupportedLanguage(string languageCode, out string normalized)
    {
        normalized = NormalizeLanguageCode(languageCode);
        return SupportedLangs.Contains(normalized);
    }





}
