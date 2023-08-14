global using global::System;
global using global::System.Collections.Generic;

global using JetBrains.Annotations;

global using ContractsPureAttribute = System.Diagnostics.Contracts.PureAttribute;

#if LESSTHAN_NET45
global using StringEx = System.StringEx;
global using ArrayEx = System.ArrayEx;
global using CharEx = System.CharEx;
global using EnumEx = System.EnumEx;
global using TaskEx = System.Threading.Tasks.TaskEx;
global using TaskExEx = System.Threading.Tasks.TaskExEx;
#else
// ReSharper disable BuiltInTypeReferenceStyleForMemberAccess
global using StringEx = System.String;
global using ArrayEx = System.Array;
global using CharEx = System.Char;
global using EnumEx = System.Enum;
global using TaskEx = System.Threading.Tasks.Task;
global using TaskExEx = System.Threading.Tasks.Task;
#endif