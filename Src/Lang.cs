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
        return languageCode.Trim().ToLowerInvariant() switch
        {
            "zh" or "zh-cn" => zh_CN,
            "en" or "en-us" => en_US,
            _ => zh_CN
        };
    }

    /// <summary>
    /// 返回所有支持的语言代码只读集合。
    /// </summary>
    /// <returns></returns>
    public static IReadOnlyCollection<string> GetAllSupportedLanguages() => SupportedLangs;

    /// <summary>
    /// 判断给定的语言代码是否为支持的语言，并返回标准化结果。
    /// </summary>
    /// <param name="languageCode"></param>
    /// <returns>Item1: 是否支持，Item2: 标准化后的语言代码</returns>
    public static (bool isSupported, string normalized) IsSupportedLanguage(string languageCode)
    {
        var normalized = NormalizeLanguageCode(languageCode);
        return (SupportedLangs.Contains(normalized), normalized);
    }

    /// <summary>
    /// 多语言插件式接口，后续可通过实现该接口扩展多语言支持。
    /// </summary>
    public interface ILanguageProvider
    {
        /// <summary>
        /// 语言代码，如 zh-CN、en-US
        /// </summary>
        string LanguageCode { get; }

        /// <summary>
        /// 获取指定key的本地化文本。
        /// </summary>
        string GetText(string key);
    }

    /// <summary>
    /// 预留：注册多语言插件（后续可实现自动扫描/动态加载）
    /// </summary>
    private static readonly List<ILanguageProvider> LanguageProviders = new();

    /// <summary>
    /// 预留：注册语言插件的方法。
    /// </summary>
    public static void RegisterLanguageProvider(ILanguageProvider provider)
    {
        if (provider != null && !LanguageProviders.Any(p => p.LanguageCode == provider.LanguageCode))
        {
            LanguageProviders.Add(provider);
        }
    }

    /// <summary>
    /// 预留：通过插件获取本地化文本（如有插件则优先使用插件）。
    /// </summary>
    public static string? GetTextFromProvider(string key, string languageCode)
    {
        var provider = LanguageProviders.FirstOrDefault(p => p.LanguageCode == NormalizeLanguageCode(languageCode));
        return provider?.GetText(key);
    }


}
