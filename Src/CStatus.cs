using System.Collections.Generic;

namespace Src;

public class CStatus : EventArgs
{
    public static readonly CStatus OK = new CStatus(ErrorInfoArgs.Empty);

    private ErrorInfoArgs errorInfoArgs = ErrorInfoArgs.Empty;

    public CStatus()
    {

    }

    public CStatus(ErrorInfoArgs args)
    {
        errorInfoArgs = args;
    }

    public bool IsOk() => string.IsNullOrEmpty(errorInfoArgs?.ErrorCode);

    public bool IsError() => !IsOk();

    private CStatus AddAssign(CStatus cur)
    {
        // 如果当前状态已经有错误信息，直接返回当前状态
        if (!IsOk() || cur.IsOk())
        {
            return this;
        }
        // 如果当前状态没有错误信息，但传入的状态有错误信息，则更新当前状态
        errorInfoArgs = cur.errorInfoArgs;
        return this;
    }

    public static CStatus operator +(CStatus a, CStatus b)
    {
        return a.AddAssign(b);
    }


}

public class ErrorInfoArgs : EventArgs
{
    public new static readonly ErrorInfoArgs Empty = new ErrorInfoArgs();

    private ErrorInfoArgs()
    {
        ErrorCode = string.Empty;
    }

    public ErrorInfoArgs(string errorCode, string? languageCode = "zh", string? message = "")
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));

        AddMessage(languageCode, message);
    }

    public string ErrorCode { get; private set; } = string.Empty;

    private readonly Dictionary<string, string> _messages = Lang.GetAllSupportedLanguages()
        .ToDictionary(lang => lang, lang => string.Empty);

    /// <summary>
    /// 根据语言代码添加错误消息。
    /// </summary>
    /// <param name="languageCode"></param>
    /// <param name="message"></param>
    /// <exception cref="ArgumentException">不支持语言</exception>
    public void AddMessage(string message, string languageCode = "zh")
    {
        if (!Lang.IsSupportedLanguage(languageCode, out var normalized))
            throw new ArgumentNullException($"UnSupportedLanguage: {languageCode} \t message: {message}");

        _messages.TryAdd(normalized, message);
    }


    /// <summary>
    /// 获取指定语言的错误消息，如果没有找到则返回默认中文（有的话），或返回外部输入的默认消息。
    /// </summary>
    /// <param name="languageCode"></param>
    /// <param name="defaultMessage"></param>
    /// <returns></returns>
    public string GetMessage(string languageCode, string defaultMessage = "Unknown error")
    {
        var normalized = Lang.NormalizeLanguageCode(languageCode);

        if (_messages.TryGetValue(normalized, out var message))
            return message;

        return _messages.Values.FirstOrDefault() ?? defaultMessage;
    }


}



