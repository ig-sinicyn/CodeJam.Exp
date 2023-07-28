# CodeJam.Light
This is personal repository for the future versions of [CodeJam](https://github.com/rsdn/CodeJam) project

CodeJam.Light project aims to ressurrect and to actualize the CodeJam library

## What is CodeJam?
CodeJam is not a framework. The library does not try to influence code style
or architecture of your project. Instead, we provide a set of helpers that
cover missing functionality in the .Net BCL and eases code development.

## What are our goals?

* No new concepts. We do follow well-established BCL code style and try
to follow least surprise design principle.
* Doogfoofing. Every and each helper that is included in the library
has to be used in real projects and has to proof its efficiency.
* Wide targeting range. We use the [Theraot.Core](https://github.com/theraot/Theraot/) library to
support all .Net versions starting with .Net 3.5
(technically, we may support .Net 2.0 but we have no test pipeline for that).

## Full targeting mode

Just to make things more interesting, we do target all major frameworks starting from Net35.

We use amazing [Theraot.Core](https://github.com/theraot/Theraot/) library for types 
that are missing from earlier .Net versions.

For the code:
1. We do use [T4 template](build/Props/CodeJam.Targeting.tt) to generate msbuild .props file with targeting build properties.
   Check the [Targeting Config](build/Props/CodeJam.Targeting.Config.ttinclude) file for our setup details
1. Current targeting limitations:
   * No .Net 1.1 support (have no sense, anyways).
   * [No class records](https://github.com/dotnet/roslyn/issues/55812) for netcoreapp1.\*, net3.5, netstandard1.5 and earlier.
   * No span support for .Net Standard 1.0 or .Net 4.0.3 and earlier (not supported by System.Memory package)
