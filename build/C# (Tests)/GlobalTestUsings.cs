global using global::System;
global using global::System.Collections.Generic;

global using JetBrains.Annotations;

global using FluentAssertions;

global using NUnit.Framework;

global using ContractsPureAttribute = System.Diagnostics.Contracts.PureAttribute;

#if LESSTHAN_NET45
global using StringEx = System.StringEx;
global using ArrayEx = System.ArrayEx;
global using TaskEx = System.Threading.Tasks.TaskEx;
#else
// ReSharper disable BuiltInTypeReferenceStyleForMemberAccess
global using StringEx = System.String;
global using ArrayEx = System.Array;
global using TaskEx = System.Threading.Tasks.Task;
#endif

#if LESSTHAN_NET40
global using AssertEx = NUnit.Framework.AssertEx;
#else
global using AssertEx = NUnit.Framework.Assert;
#endif
