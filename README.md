# DevExtreme FilterBuilder

This library provides a single class which turns a [specification object](https://en.wikipedia.org/wiki/Specification_pattern) produced by [DevExtreme's FilterBuilder component](https://js.devexpress.com/Documentation/ApiReference/UI_Components/dxFilterBuilder/) into a [LINQ Expression](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.expression) that can be used by any LINQ provider.

It provides the follwing features:

* Support for all built-in operators.
* Support for custom condition operators.
* Support for custom condition parameter parsing
* A default parameter parser for simple types (strings, numbers, enums).

![Nuget version](https://img.shields.io/nuget/vpre/DevExtremeFilterBuilder) ![Nuget](https://img.shields.io/nuget/dt/DevExtremeFilterBuilder)

There are three kinds of expressions that are encoded:
1. Condition Expressions - take the value of a property of an object and compare it to some other value(s).
2. Group Expressions - which combine other expressions with logical AND/OR.
3. Not Expressions - which simply negate another expression.

## Basic usage

The user may specify a filter that gets all products where the cost is greater than or equal to 100.

The property we wish to filter on is `Cost`, the operator is `>=` and the parameter is `100`.

This filter would be output by the DevExtreme FilterBuilder component as the following JSON string:

```json
["Cost", ">=", 100]
```

To turn this JSON string into a filter, we provide the string to the FilterBuilder class, which returns an `Expression`. This is passed directly to a LINQ provider (i.e. an `IQueryable`) which performs the filtering.

```csharp
var filterBuilder = new FilterBuilder();

var costGreaterThanOrEqualTo100 = filterBuilder.GetExpression<Product>(@"[""Cost"", "">="", 100]").Compile();

var expensiveProducts = allProducts.Where(costGreaterThanOrEqualTo100)
```


## Register a custom condition parameter parser

The specification object provides parameters as a JSON Element. These elements have default conversions to CLR values (e.g. `Number` -> `Double`), but for custom types they will likely require some parsing.

```json
["Cost", ">=", "10USD"]
```

The following c# snippet shows how to parse the parameter for a condition against the `Cost` property.
```csharp
filterBuilder.RegisterParser("Cost", el => Money.Parse(el.GetString()));
```

Depending on the operator, this parameter might be encoded as an array, other times as a single string - to make your parser more flexible it might make sense to customise the logic as appropriate. In this case I've opted to provide either a single `ProductCategory` or an array of `ProductCategory` depending on the JSON type.

```csharp
filterBuilder.RegisterParser("Category", el =>
{
    if (el.ValueKind == System.Text.Json.JsonValueKind.Array)
        return el.EnumerateArray()
            .Select(x => x.GetString())
            .Select(x => Enum.Parse<ProductCategory>(x))
            .ToArray();

    else if (el.ValueKind == System.Text.Json.JsonValueKind.String)
        return Enum.Parse<ProductCategory>(el.GetString());

    else
        throw new ArgumentOutOfRangeException();

});
```


## Register a custom condition operator

Within a condtion expression there are three elements:
1. The object property
2. The operator
3. The parameter

DevExtreme Filter Builder supports the ability to define your own operators. For example, the anyof operator which checks that `Category` is any of the provided values.

```json
["Category", "anyof", ["Food", "Furniture"]]
```

In order to register a custom operator, we need to define two things:
1. The name of the operator (e.g. "anyof")

2. The operator implementation.
   This is a `Func` which takes the value of the property, and the value of the parameter, performs some calculation, and returns a boolean result.

Because the property value and parameter is typed as an object, it might be necessary to cast them to some other type first.

```csharp
filterBuilder.RegisterOperator("anyof", (object value, object parameter) =>                   
{
   var allowedValues = ((object[])parameter).Select(x => x.ToString());
   var typedValue = value.ToString();
   return allowedValues.Contains(typedValue);
});
```

You may find that an operator implementation you develop can not be translated by the Linq provider. In this case you need load the entities into memory and use the compiled Expression. See [Client versus Server Evaluation](https://docs.microsoft.com/en-us/ef/core/querying/client-eval) for more.

```csharp
var jsonFilter = "...";
var predicate = builder.GetExpression<Product>(jsonFilter).Compile();
var actualList = dbContext.Products.AsEnumerable().Where(predicate);
```

Alternatively, you can directly supply the Expression:

```csharp
FilterBuilder builder = new();

builder.RegisterOperator("anyof",
(Expression value, Expression parameter) =>
{
    var method = typeof(Enumerable)
                    .GetMethods()
                    .Where(m => m.Name == "Contains")
                    .Single(m => m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(object));

    return Expression.Call(method, parameter, Expression.TypeAs(value, typeof(object));
});
```

Note that the property value will be supplied with the property type, so you may need to cast it to some other type in order for the Expression to be correctly evaluated.
