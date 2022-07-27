# ArgParseSharp
![CLI-icon](icon40x40.png) *Version: 0.1.0* 

 A simple and ready to use command-line argument parser for .NET Standard

You describe what you want from the arguments and `ArgParseSharp` will care to try to extrapolate all the information from your command line arguments.

### Preparation:
```c#
using ArgParseSharp;

class Options {
    [Argument.Version]
    public bool Version { get; set; }
    [Argument.Boolean]
    public bool MyFlag { get; set; }
    [Argument.String]
    public string Input { get; set; }
    [Argument.Int]
    public int Index { get; set; }
}

// ...
// We will assume these args:
string[] args = new string[] {
    "-version",
    "-myflag",
    "-input", "asd",
    "-index", "42",
    "-myExtra", "extra"
};
```

### Automatic parsing:
```c#
// by omitting args parameter or passing null
// the default program arguments are used
Options options = ArgumentParser.AutoParse<Options>(args);

// USAGE EXAMPLE:
Console.WriteLine($"Version: {options.Version}"); // Version: True
Console.WriteLine($"MyFlag: {options.MyFlag}");   // MyFlag: True
Console.WriteLine($"Input: {options.Input}");     // Input: asd
Console.WriteLine($"Index: {options.Index}");     // Index: 42
```

### Generate an `ArgumentParser` with automatic inference:
```c#
ArgumentParser parser = ArgumentParser.FromType<Options>();
// OR 
ArgumentParser parser = ArgumentParser.FromType(typeof(Options));
```

### Parsing arguments without a reference class:
```c#
ParsedArguments parsed = parser.ParseArgs(args);

// USAGE EXAMPLE:
foreach (ParsedElement element in parsed.Values) {
    Console.WriteLine(element.ToString());
    Console.WriteLine($"\t(same as {element.Name}={element.Value} ({element.Argument})));
}
/* OUTPUT:
--version=True (-version)
    (same as --version=True (-version))
MyFlag=True (-myflag)
    (same as MyFlag=True (-myflag))
Input=asd (-input asd)
    (same as Input=asd (-input asd))
Index=42 (-index 42)
    (same as Index=42 (-index 42))
help=False ()
    (same as help=False ())
*/
```

### Arguments not recognized:
```c#
foreach (ParsedElement extra in parsed.Extras) {
    Console.WriteLine(extra.ToString());
}
/* OUTPUT:
-myExtra= ()
extra= ()
*/

// To retrieve extra in autoparse mode:
Options options = ArgumentParser.AutoParse<Options>(out ParsedElement[] extras, args);
```

### Retrieving arguments from a `Stream`:
```c#
parser.ParseArgs(stream);
// OR
ArgumentParser.AutoParse<Options>(stream);
// OR
ArgumentParser.AutoParse<Options>(out ParsedElement[] extras, stream);
```
### Automatic help argument:
```c#
// if -h flag is presnt:
parser.ParseArgs("-h");
// the program will automatically
// print to console the help message
// and exit with code 0

// Removing automatic help handling
parser.AddHelp = false;
```

### Print Help and Usage:
```c#
/// HELP
// string
string help = parser.PrintHelp();
// Console
parser.PrintHelp(null);
// Stream
parser.PrintHelp(stream);

/// USAGE
// string
string usage = parser.PrintUsage();
// Console
parser.PrintUsage(null);
// Stream
parser.PrintUsage(stream);
```


## Customization:
### Output Formatting:
```c#
// MyCustomHelpFormatter : IHelpFormatter
parser.HelpFormatter = new MyCustomHelpFormatter();
// MyCustomUsageFormatter : IUsageFormatter
parser.UsageFormatter = new MyCustomUsageFormatter();

// MyCustomFormatter : IFormatter
// IFormatter : IHelpFormatter, IUsageFormatter
parser.Formatter = new MyCustomFormatter();
// this is a shorthand for
IFormatter myCustomFormatter = new MyCustomFormatter();
parser.HelpFormatter = myFormatter;
parser.UsageFormatter = myFormatter;
```

### Argument Parsing - Custom Argument:
```c#
struct MyArgumentValue {
    public string Name;
    public int Value;
    public MyArgumentValue(string name, int value) {
        Name = name;
        Value = value;
    }
}
// The generic Argument<T> helps to manage parsing values
// (eg. automatically infer the Parser delegate: T Parser(string))
class MyArgument : Argument<MyArgumentValue> {
    // constructor with custom parser (and automatic default)
    public MyArgument(string name, Parser? type = null) :
        base(name, type == null ? DefaultParser : type) { }
    // constructor with constant
    public MyArgument(string name, MyArgumentValue constant) :
        base(name, constant) { }
    // constructor with built constant
    public MyArgument(string name, string argName, int argValue) :
        base(name, new MyArgumentValue(argName, argValue)) { }

    // default parser for this argument
    public static MyArgumentValue DefaultParser(string value) {
        string[] splitted = value.Split(':');
        if (splitted.Length < 2) { return default(MyArgumentValue); }
        string argName = splitted[0];
        int.TryParse(splitted[1], out int argVal);
        return new MyArgumentValue(argName, argVal);
    }
}

// USAGE EXAMPLE:
parser.AddArgument(new MyArgument("-myArg"));
// call constructor with custom parser
parser.AddArgument<MyArgument>("-myArg"/*, null*/);
// call constructor with constant
parser.AddArgument<MyArgument>("-myArg", new MyArgumentValue("test", 3));
// constructor with built constant
parser.AddArgument<MyArgument>("-myArg", "test", 3);

```

### Argument Parsing - Custom Attribute:
```c#
// Suggested Attribute Usage
[AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = false
)]
class MyAttribute : ArgumentAttribtue {
    // The type the parser should return
    protected override Type _resultType { get; } = typeof(MyArgumentValue);
    // The delegate type of the parser
    protected override Type _delegateType { get; } = typeof(MyArgument.Parser);
    // default constructor
    public MyAttribute() : base(MyArgument.DefaultParser) { }
    // constant constructor
    // (NOTE: only certain arguments type are available in Attribute constructors)
    public MyAttribute(string argName, int argValue) :
        base(new MyArgumentValue(argName, argValue)) { }
    // Parser (delegate) retriever constructor
    public MyAttribute(Type parserOwner, string? parserName = null) :
        base(parserOwner, parserName) { }

    // We tell base class to create an instance of our custom argument (parser)
    protected override Argument CreateArgument(string name, Delegate? parser) {
        return new MyArgument(name, (MyArgument.Parser?)parser);
    }
    // We tell base class to create an instance of our custom argument (constant)
    protected override Argument CreateArgument(string name, Nullable<object> consant) {
        return new MyArgument(name, consant.As<MyArgumentValue>());
    }
}

// USAGE EXAMPLE:
class MyOptions {
    [MyAttribute]
    public MyArgumentValue MyValue { get; set; }
}
```
### Argument Reading (Stream only):
```c#
class MyCustomArgumentParser : ArgumentParser {
    protected override string ReadStream(Stream target) {
        // code for read a string from the string
    }

    protected override string[] ConvertArgLineToArgs(string argLine) {
        // code for split the string into arguments
    }
}
```