using System;
using Minerals.AutoInterfaces;

namespace Minerals.Examples
{
    public interface ITestClass
    {
        EventHandler? Event1 { get; set; }
        Action<int>? Event2 { get; set; }
    }
}
