using System;
using Minerals.AutoInterfaces;

namespace Minerals.Examples
{
    public interface ITestClass
    {
        event EventHandler? Event1;
        event Action<int>? Event2;
    }
}
