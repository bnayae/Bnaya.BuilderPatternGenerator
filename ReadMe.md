# Builder Pattern Code Generation for .NET

[![NuGet](https://img.shields.io/nuget/v/BuilderPatternGenerator.svg)](https://www.nuget.org/packages/BuilderPatternGenerator/)  
[![Deploy](https://github.com/bnayae/Bnaya.BuilderPatternGenerator/actions/workflows/build-publish-v2.yml/badge.svg)](https://github.com/bnayae/Bnaya.BuilderPatternGenerator/actions/workflows/build-publish-v2.yml)

This tool is **designed to simplify the process of creating builders pattern** for your classes, structs, or records. Unlike traditional builder generation tools, our approach focuses on immutability and ease of use. We've incorporated features that make building objects even more convenient and user-friendly.

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Examples](#examples)

## Introduction

The Builder Pattern Code Generation tool is designed with the modern developer in mind. It automates the process of creating builder patterns for your types while promoting immutability and a user-friendly experience. The tool exposed steps of building the mandatory information before proceeding to optional steps, ensuring your objects are built with all the necessary data.

## Features

- **Immutability**: The generated builders follow an immutable approach, ensuring the integrity of your objects, and reusable of a builder for creating a multiples lightly different objects. 
- **Step-by-Step Building**: The builder guides you through the process, requiring mandatory fields before proceeding to optional ones.
- **Hidden Used Stages**: Already used stages are hidden, streamlining the building process and reducing complexity.
- **Clean and Readable Code**: The generated code is well-organized and easy to understand, making maintenance a breeze.
- **Saves Development Time**: Automating builder creation saves you time and effort, allowing you to focus on Type's data rather on the Builder techniques.

## Getting Started

To start using the Builder Pattern Code Generation tool, follow these simple steps:

1. **Installation**: Include the NuGet of the builder generation and the abstraction libraries:

```bash
dotnet add package BuilderPatternGenerator 
dotnet add package BuilderPatternGenerator.Abstractions
```

2. **Decorate Your Type**: Mark the class, struct, or record you want to generate a builder for with the `[GenerateBuilderPattern]` attributes. This signals the generator to create the builder for your type.

## Usage

Using the generated builder is intuitive and straightforward. Here's a basic example:

```cs
// PersonBuilder.cs
[GenerateBuilderPattern]
public partial record PersonBuilder(int Id, string Name)
{
    public required string Email { get; init; }
    public DateTime Birthday { get; init; }
}
```

> Don't forget to mark the Type as `partial`

```cs
[Fact]
public void PersonBuilder_Test()
{
    DateTime dateTime = DateTime.Now.AddYears(-32);
    var p1 = PersonBuilder.CreateBuilder()
                    .AddName("Joe")
                    .AddId(3)
                    .AddEmail("joe16272@gmail.com")
                    .AddBirthday(dateTime)
                    .Build();

    Assert.Equal(p1, new PersonBuilder(3, "Joe") { Email = "joe16272@gmail.com", Birthday = dateTime });
}
```

## Examples

For more comprehensive examples, check out the `Bnaya.BuilderPatternGenerator.SrcGen.Playground` project in our GitHub repository. These examples cover various scenarios, demonstrating the power and flexibility of our  Builder Pattern Code Generation tool.

