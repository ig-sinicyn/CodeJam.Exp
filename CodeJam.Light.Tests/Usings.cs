global using global::System;
global using global::System.Collections.Generic;
global using global::System.Linq;

global using JetBrains.Annotations;

global using FluentAssertions;

global using NUnit.Framework;

global using ContractsPureAttribute = System.Diagnostics.Contracts.PureAttribute;

#if NET45_OR_GREATER || TARGETS_NETCOREAPP
global using TaskEx = System.Threading.Tasks.Task;
#elif NET40_OR_GREATER
global using TaskEx = System.Threading.Tasks.TaskEx;
#else
global using TaskEx = System.Threading.Tasks.Task;
#endif