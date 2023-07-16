global using global::System;
global using global::System.Collections.Generic;

global using JetBrains.Annotations;

global using ContractsPureAttribute = System.Diagnostics.Contracts.PureAttribute;

#if NET40_OR_GREATER || TARGETS_NETSTANDARD || TARGETS_NETCOREAPP
global using StringEx = System.String;
global using ArrayEx = System.Array;
// ReSharper disable BuiltInTypeReferenceStyleForMemberAccess
#else
global using StringEx = System.StringEx;
global using ArrayEx = System.ArrayEx;
#endif