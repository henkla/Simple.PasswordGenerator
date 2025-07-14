
# Simple.PasswordGenerator

[![GitHub Repo stars](https://img.shields.io/github/stars/henkla/Simple.PasswordGenerator)](https://github.com/henkla/Simple.PasswordGenerator/stargazers)  
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/henkla/Simple.PasswordGenerator/nuget-package.yml)](https://github.com/henkla/Simple.PasswordGenerator/actions)  
[![NuGet version](https://img.shields.io/nuget/v/Simple.PasswordGenerator.svg?style=flat-square)](https://www.nuget.org/packages/Simple.PasswordGenerator/)  
[![NuGet Downloads](https://img.shields.io/nuget/dt/Simple.PasswordGenerator)](https://www.nuget.org/packages/Simple.PasswordGenerator/)  
[![GitHub Issues](https://img.shields.io/github/issues/henkla/Simple.PasswordGenerator)](https://github.com/henkla/Simple.PasswordGenerator/issues)

---

## 🚀 Overview

**Simple.PasswordGenerator** is a lightweight, secure password generator for .NET applications.  
It creates cryptographically strong passwords based on customizable policies — including length, required character types, and special characters.

No dependencies beyond .NET. Simple to integrate, flexible to configure, and safe to use.

---

## 🔑 Key Features

- 🔐 Cryptographically secure password generation
- ⚙️ Fully customizable policies (length, uppercase, lowercase, digits, special chars)
- 🎯 Default policy with sensible security defaults
- 🔄 Supports configuration via lambda or predefined policy objects
- 🧩 Compatible with .NET 8 and later

---

## 📚 Table of Contents

1. [Getting Started](#getting-started)
    - [Installing](#installing)
    - [Basic Usage](#basic-usage)
    - [Advanced Usage](#advanced-usage)
2. [Examples](#examples)
3. [Technical Information](#technical-information)
4. [Known Issues & Limitations](#known-issues--limitations)
5. [Contributing](#contributing)
6. [License](#license)

---

## 🚦 Getting Started

Use **Simple.PasswordGenerator** to generate strong, policy-compliant passwords easily.

### 📦 Installing

Add the package via NuGet:

```bash
dotnet add package Simple.PasswordGenerator
```

---

### 🧪 Basic Usage

```csharp
var generator = new PasswordGenerator();
var password = generator.Generate();
Console.WriteLine($"Generated password: {password}");
```

---

### 🎯 Advanced Usage

Configure a custom password policy using a lambda:

```csharp
var password = generator.Generate(policy =>
{
    policy.Length = 20;
    policy.RequireSpecial = false;
    policy.RequireDigit = true;
    policy.RequireUppercase = true;
    policy.RequireLowercase = true;
});
```

Or generate using a predefined `PasswordPolicy` object:

```csharp
var policy = new PasswordPolicy
{
    Length = 12,
    RequireSpecial = true,
    RequireDigit = true,
    RequireUppercase = false,
    RequireLowercase = true,
    SpecialCharacters = "@#$"
};
var passwordFromPolicy = generator.Generate(policy);
```

---

## 🖼️ Examples

```csharp
using Simple.PasswordGenerator;

var passwordGenerator = new PasswordGenerator();

// Default policy password
var defaultPassword = passwordGenerator.Generate();
Console.WriteLine($"Default policy password: {defaultPassword}");

// Custom policy via lambda
var customPassword = passwordGenerator.Generate(policy =>
{
    policy.Length = 20;
    policy.RequireSpecial = false;
    policy.RequireDigit = true;
    policy.RequireUppercase = true;
    policy.RequireLowercase = true;
});
Console.WriteLine($"Custom policy password: {customPassword}");

// Using a PasswordPolicy instance
var policyInstance = new PasswordPolicy
{
    Length = 12,
    RequireSpecial = true,
    RequireDigit = true,
    RequireUppercase = false,
    RequireLowercase = true,
    SpecialCharacters = "@#$"
};
var policyPassword = passwordGenerator.Generate(policyInstance);
Console.WriteLine($"Password from policy instance: {policyPassword}");
```

---

## 🔬 Technical Information

- Uses `System.Security.Cryptography.RandomNumberGenerator` for secure randomization
- Password policy validation ensures minimum length and at least one character from required categories
- Character categories: uppercase, lowercase, digits, and customizable special characters
- Password characters shuffled with Fisher–Yates algorithm for unpredictability

---

## 🐞 Known Issues & Limitations

- Does not support password strength estimation or entropy reporting

---

## 🤝 Contributing

Bug reports, feature requests, and pull requests are welcome!  
Please open an issue or submit a PR via [GitHub Issues](https://github.com/henkla/Simple.PasswordGenerator/issues).

---

## 📄 License

This project is licensed under the MIT License.
