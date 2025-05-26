# Minerals.AutoInterfaces

This NuGet package provides a functionality to automatically generate interfaces for C# classes with a single attribute. This simplifies the creation of interfaces for classes with clearly defined public members, without having to manually write interface code.

## Features

- **Automatic interface generation:** Saves time and reduces the risk of errors when creating interfaces for classes.
- **Support for generic methods and constraints:** Allows for generating interfaces for complex classes with generic methods.
- **Support for custom getters and setters:** Generates interfaces for properties with custom getter and setter implementations.
- **Customizable interface name:** Allows you to name the interface according to naming conventions or user preferences.
- **Compatible with .NET Standard 2.0 and C# 7.3+:** Works on a wide range of platforms and development environments.

## Installation

Add the Minerals.AutoInterfaces nuget package to your C# project using the following methods:

### 1. Project file definition

```xml
<PackageReference Include="Minerals.AutoInterfaces" Version="0.2.0" />
```

### 2. dotnet command

```bat
dotnet add package Minerals.AutoInterfaces
```

## Changes in ``0.2.0`` version

The new name for the marker attribute is ``Minerals.AutoInterfaces.AutoInterface``

```csharp
// OLD MARKER ATTRIBUTE NAME - v0.1.6
[Minerals.AutoInterfaces.GenerateInterface]
public class TestClass
{
    public int Property1 { get; set; }
}

// NEW MARKER ATTRIBUTE NAME - v0.2.0
[Minerals.AutoInterfaces.AutoInterface]
public class TestClass
{
    public int Property1 { get; set; }
}
```

## Usage

To use the package, add the ``[AutoInterface]`` marker attribute to the selected class or struct.

```csharp
using System;

namespace TestNamespace
{
    [Minerals.AutoInterfaces.AutoInterface]
    public class TestClass
    {
        public int Property1 { get; set; }
        public int Property2 { get; private set; }
        protected int Property3 { get; set; }
        public event Action? Event1;

        public void Method1()
        {
            Console.WriteLine("Hello World");
        }

        public void Method2(int arg)
        {
            Console.WriteLine(arg);
        }

        public bool Method3<T0, T1>(T0 arg0, T1 arg1) where T0 : IEquatable<T1> where T1 : IEquatable<T0>
        {
            return arg0.Equals(arg1);
        }
    }
}
```

The code above will generate the ``ITestClass.g.cs`` file with the ``ITestClass`` interface.

```csharp
namespace global::TestNamespace
{
    [global::System.Runtime.CompilerServices.CompilerGenerated]
    public interface ITestClass
    {
        int Property1 { get; set; }
        int Property2 { get; }
        event Action? Event1;
        void Method1();
        void Method2(int arg);
        bool Method3<T0, T1>(T0 arg0, T1 arg1) where T0 : global::System.IEquatable<T1> where T1 : global::System.IEquatable<T0>;
    }
}
```

### Package supports custom interface names

```csharp
namespace TestNamespace
{
    [Minerals.AutoInterfaces.AutoInterface("ITestInterface")]
    public class TestClass
    {
        public int Property1 { get; protected set; } = 1;
    }
}
```

The code above will generate the ``ITestInterface.g.cs`` file with the ``ITestInterface`` interface.

```csharp
namespace global::TestNamespace
{
    [global::System.Runtime.CompilerServices.CompilerGenerated]
    public interface ITestInterface
    {
        int Property1 { get; }
    }
}
```

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [branches on this repository](https://github.com/SzymonHalucha/Minerals.AutoInterfaces/branches).

## Authors

- **Szymon Hałucha** - Maintainer

See also the list of [contributors](https://github.com/SzymonHalucha/Minerals.AutoInterfaces/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](./LICENSE) file for details.
