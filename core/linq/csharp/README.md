# 101 C# LINQ Samples

Learn how to use LINQ in your applications with these code samples, covering the entire range of LINQ functionality and demonstrating LINQ
to objects, and LINQ to XML.

## aggregate

Learn how to generate a single value from a sequence

## concatenation

Combine multiple sequences into a single source sequence.

## conversion

Convert sequences of one type into another.

## customsequence

Learn how to build seqeunces from multiple inputs.

## element

Use the index of items in a sequence during a query.

## equality

Compare sequences for equality.

## generation

Generate sequences by running a query.

## grouping

Group items into sub-sequences.

## join

Combine multiple sequences into a single sequence
by matching values on properties.

## ordering

Sort sequences.

## partitioning

Dividing an input sequence into two parts without rearranging
them and then returning one of the parts.

## projection

Transform sequences into new types.

## quantifier

Test for all or any elements matching a condition.

## queryexecution

Learn about deferred and immediate query execution.

## restriction

Filter sequences using `Where`.

## setoperators

Learn about set operations such as `Distinct`, `Except`, `Intersect`,
and `Union`.

## Build and Run

To build and run the sample, type the following two commands in any of the subdirectories:

```
dotnet restore
dotnet run
```

`dotnet restore` restores the dependencies for this sample.

`dotnet run` builds the sample and runs the output assembly.

**Note:** Starting with .NET Core 2.0 SDK, you don't have to run [`dotnet restore`](https://docs.microsoft.com/dotnet/core/tools/dotnet-restore) because it's run implicitly by all commands that require a restore to occur, such as `dotnet new`, `dotnet build` and `dotnet run`. It's still a valid command in certain scenarios where doing an explicit restore makes sense, such as [continuous integration builds in Azure DevOps Services](https://docs.microsoft.com/azure/devops/build-release/apps/aspnet/build-aspnet-core) or in build systems that need to explicitly control the time at which the restore occurs.
