global using global::System;
global using global::System.Collections.Generic;

global using JetBrains.Annotations;

global using ContractsPureAttribute = System.Diagnostics.Contracts.PureAttribute;

#if NET45_OR_GREATER || TARGETS_NETSTANDARD || TARGETS_NETCOREAPP
// ReSharper disable BuiltInTypeReferenceStyleForMemberAccess
global using StringEx = System.String;
global using ArrayEx = System.Array;
global using TaskEx = System.Threading.Tasks.Task;
#else
global using StringEx = System.StringEx;
global using ArrayEx = System.ArrayEx;
global using TaskEx = System.Threading.Tasks.TaskEx;
#endif