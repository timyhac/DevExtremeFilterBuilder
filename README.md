# DevExtreme FilterBuilder

This library provides a single class which turns a [specification object](https://en.wikipedia.org/wiki/Specification_pattern) produced by DevExtreme's FilterBuilder into a [LINQ Expression](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.expression) that can be used by any LINQ provider.

It provides the follwing features:

* Support for all built-in operators with simple types.
* Support for custom operators.


## Basic usage

```csharp
var filterBuilder = new FilterBuilder();
var costGreaterThan10 = filterBuilder.Get<Product>(@"[""Cost"", "">="", 100]").Compile();

var expensiveProducts = allProducts.Where(costGreaterThan100)
```

## Register a custom condition operator

DevExtreme Filter Builder supports the ability to define your own operators.

In order to register a custom operator, we need to define three things:
1. The name of the operator (e.g. "anyof")

2. The operator implementation.
   This is a `Func` which takes the value of the property, and the value of the parameter, performs some calculation, and returns a boolean result.

3. Parsing of the parameter into a CLR object.
   This is a `Func` used to turn the JSON object into the parameter.
