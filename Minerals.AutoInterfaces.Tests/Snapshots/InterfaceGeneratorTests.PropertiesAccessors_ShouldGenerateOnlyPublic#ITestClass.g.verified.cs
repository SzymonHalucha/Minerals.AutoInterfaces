using System;
using Minerals.AutoInterfaces;

namespace Minerals.Examples
{
    public interface ITestClass
    {
        int Property1 { get; set; }
        int Property2 { get; }
        int Property3 { set; }
        int Property4 { get; }
        string Property5 { get; init; }
        float Property6 { get; }
    }
}
