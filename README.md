# HatTrick.Reflection

[![NuGet](https://img.shields.io/nuget/v/HatTrick.Reflection.svg)](https://www.nuget.org/packages/HatTrick.Reflection/)
[![License: Apache-2.0](https://img.shields.io/badge/license-Apache--2.0-blue.svg)](LICENSE)

Reflect properties, fields, or dictionary values off any object using dotted-path expressions, with optional compiled-delegate helpers for hot paths.

**[NuGet](https://www.nuget.org/packages/HatTrick.Reflection/)** | **[hattricklabs.com](https://hattricklabs.com)**

---

## Installation

```bash
dotnet add package HatTrick.Reflection
```

## Quick Start

```csharp
using HatTrick.Reflection;

var person = new Person
{
    FirstName = "Charlie",
    BillingAddress = new Address { City = "Plano", State = "TX" }
};

// Properties, fields, and dotted paths through nested objects.
string firstName = person.ReflectItem<string>("FirstName");
string city = person.ReflectItem<string>("BillingAddress.City");

// Works on dictionaries and anonymous types too — same expression syntax.
var dict = new Dictionary<string, object> { ["Name"] = "Jorge" };
string name = dict.ReflectItem<string>("Name");

// Returns null instead of throwing when a path segment doesn't exist.
string middleName = person.ReflectItem<string>("MiddleName", throwOnNoItemExists: false);

// Register a compiled delegate for a (type, expression) pair to skip reflection on hot paths.
ReflectionHelper.Expression.RegisterHelper<Person>("BillingAddress.City", p => p.BillingAddress?.City);
city = person.ReflectItem<string>("BillingAddress.City"); // now resolved via the delegate
```

---

## Features

- Reflect properties, fields, or dictionary values through dotted-path expressions, recursing through nested objects
- Works uniformly across POCOs, anonymous types, and `IDictionary<string, object>`
- `RegisterHelper<T>` swaps in a compiled delegate per type/expression pair, bypassing reflection entirely on hot paths
- Configurable null-or-throw behavior (`NoItemExistsException`) when a path segment doesn't resolve
- Guards against runaway recursive paths via `RecursionStackDepthException` (max depth 16)

---

## License

Apache-2.0 — see [LICENSE](LICENSE).
