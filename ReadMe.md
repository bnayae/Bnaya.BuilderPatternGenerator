# Builder Pattern Code Generation for .NET



This tool is **designed to simplify the process of creating builders pattern** for your classes, structs, or records. Unlike traditional builder generation tools, our approach focuses on immutability and ease of use. We've incorporated features that make building objects even more convenient and user-friendly.

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Examples](#examples)
- [Advanced Usage](#advanced-usage)
- [Contributing](#contributing)
- [License](#license)

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

1. **Installation**: Include the builder generation library in your project. You can either download the package from [NuGet](https://www.nuget.org/) or include it as a project reference.

2. **Decorate Your Type**: Mark the class, struct, or record you want to generate a builder for with the appropriate attributes. This signals the generator to create the builder for your type.

3. **Configure Builder**: Use the provided configuration options to set up the builder's behavior, such as naming conventions and visibility modifiers.

## Usage

Using the generated builder is intuitive and straightforward. Here's a basic example:

```csharp
// Import the necessary namespaces
using YourNamespace.Builders;

// ...

// Create a new builder instance
var personBuilder = PersonBuilder.Create();

// Provide mandatory fields
var person = personBuilder
    .WithName("John Doe")
    .WithAge(30)
    .BuildMandatory();

// Now you can provide optional information
personBuilder.WithEmail("john@example.com");
personBuilder.WithPhone("+1234567890");
var completePerson = personBuilder.Build();
```

## Examples

For more comprehensive examples, check out the `Examples` directory in our GitHub repository. These examples cover various scenarios, demonstrating the power and flexibility of our Immutable Builder Code Generation tool.

## Advanced Usage

Our tool offers advanced configuration options that allow you to customize the generated code further. You can tailor the builder's behavior to match your project's specific requirements. For detailed information, refer to the [Advanced Usage](advanced-usage.md) guide.

## Contributing

We welcome contributions from the community! If you encounter any issues, have suggestions for improvements, or want to contribute in any way, please read our [Contribution Guidelines](contributing.md) for details on how to get started.

## License

This project is licensed under the [MIT License](license.md). Feel free to use, modify, and distribute it according to the terms of the license.

---

We hope our Immutable Builder Code Generation tool simplifies your development process and enhances the maintainability of your code. If you have any questions, concerns, or feedback, please don't hesitate to reach out to us. Happy coding!
