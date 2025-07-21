using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Src;
public abstract class GElement
{
    /// <summary>
    /// 依赖本节点的后继节点
    /// </summary>
    protected ConcurrentBag<GElement> RunBefore { get; } = new();

    /// <summary>
    /// 本节点依赖的前驱节点
    /// </summary>
    protected ConcurrentBag<GElement> Dependence { get; } = new();

    /// <summary>
    /// 本节点的标记名称
    /// </summary>
    protected virtual string _elementName { get; set; } = string.Empty;

    /// <summary>
    /// 元素执行次数
    /// </summary>
    private int _loopCount = 1;

    /// <summary>
    /// 当值为0的时候，即可以执行该element
    /// </summary>
    private int _leftDependCounter = 0;

    /// <summary>
    /// 元素初始化方法，通常在元素被添加到工作流中时调用。
    /// </summary>
    /// <returns></returns>
    protected internal virtual CStatus Init() => CStatus.OK;

    /// <summary>
    /// 元素核心执行方法，通常在工作流运行时调用。
    /// </summary>
    /// <returns></returns>
    protected internal abstract CStatus Run();

    /// <summary>
    /// 元素内部循环执行方法，通常在工作流运行时调用。
    /// </summary>
    /// <returns></returns>
    protected internal CStatus RunLoop()
    {
        var status = CStatus.OK;
        for (int i = 0; i < _loopCount && !status.IsError(); i++)
        {
            status += Run();
        }
        return status;
    }

    /// <summary>
    /// 元素销毁方法，通常在工作流结束时调用。
    /// </summary>
    /// <returns></returns>
    protected internal virtual CStatus Destroy()
    {
        return new CStatus();
    }

    protected string GetName() => _elementName ?? GetType().Name;


    internal void AddElementInfo(IEnumerable<GElement> depends, string name, int loop)
    {
        foreach (var depend in depends)
        {
            Dependence.Add(depend);
            depend.RunBefore.Add(this);
        }

        ResetDepend();
        _elementName = name;
        _loopCount = loop;
    }

    /// <summary>
    /// 重置依赖计数器，通常在元素被添加到工作流中时调用。
    /// </summary>
    internal void ResetDepend()
    {
        Interlocked.Exchange(ref _leftDependCounter, Dependence.Count);
    }

    /// <summary>
    /// 工作流运行时调用，表示当前元素的依赖计数器减1。
    /// </summary>
    /// <returns></returns>
    internal bool DecrementDepend()
    {
        return Interlocked.Decrement(ref _leftDependCounter) <= 0;
    }
}
