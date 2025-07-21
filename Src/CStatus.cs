using System.Collections.Generic;

namespace Src;

public class CStatus
{
    public static readonly CStatus OK = new CStatus(ErrorInfoArgs.Empty);

    private ErrorInfoArgs errorInfoArgs = ErrorInfoArgs.Empty;

    public CStatus() { }

    public CStatus(ErrorInfoArgs args)
    {
        errorInfoArgs = args;
    }

    public bool IsOk() => errorInfoArgs?.ErrorCode?.IsOk ?? true;

    public bool IsWarningOrInfo() => errorInfoArgs?.ErrorCode?.IsWarningOrInfo ?? true;

    public bool IsError() => errorInfoArgs?.ErrorCode?.IsError ?? false;

    private CStatus AddAssign(CStatus other)
    {
        if (IsError()) return this;
        if (other.IsError()) return other;
        return this; // 双方都OK时返回任意
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
        ErrorCode = new ErrorCode();
    }

    public ErrorInfoArgs(ErrorCode errorCode, string? message = "", string? languageCode = "zh")
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));

        AddMessage(message, languageCode);
    }

    public ErrorCode ErrorCode { get; private set; } = new();

    /// <summary>
    /// Dic to store langTag and message pairs.
    /// </summary>
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

        _messages[normalized] = message;
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


public record ErrorCode(string strErrorCode = "DefaultErrorCode", ErrorCodeType ErrorCodeType = ErrorCodeType.Ok)
{
    public bool IsOk => ErrorCodeType == ErrorCodeType.Ok;
    public bool IsWarningOrInfo => ErrorCodeType == ErrorCodeType.Warning || ErrorCodeType == ErrorCodeType.Info;
    public bool IsError => ErrorCodeType == ErrorCodeType.Error;
}

public enum ErrorCodeType
{
    Ok = 0,
    Info = 1,
    Warning = 2,
    Error = 3,
}



