# ArgParseSharp
![CLI-icon](icon40x40.png) *Version: 0.1.0* 

 A simple and ready to use command-line argument parser for .NET Standard

You describe what you want from the arguments and `ArgParseSharp` will care to try to extrapolate all the information from your command line!

## Index
- [Basics](#basics)
    - [Preparation](#preparation)
    - [Automatic Parsing](#automatic-parsing)
    - [Generate an ArgumentParser with automatic inference](#generate-an-argumentparser-with-automatic-inference)
    - [Parsing arguments without a reference class](#parsing-arguments-without-a-reference-class)
    - [Read non-recognized arguments](#read-non-recognized-arguments)
    - [Retrieving arguments from a Stream](#retrieving-arguments-from-a-stream)
    - [Automatic help argument](#automatic-help-argument)
    - [Print Help and Usage](#print-help-and-usage)
- [Parsing Directives](#parsing-directives)
    - [Automatic inference](#automatic-inference)
    - [Disable automatic inference](#disable-automatic-inference)
    - [Useful properties for arguments and casting](#useful-properties-for-arguments-and-casting)
- [Customization](#customization)
    - [Output Formatting](#output-formatting)
    - [Argument Parsing - Custom Argument](#argument-parsing---custom-argument)
    - [Argument Parsing - Custom Attribute](#argument-parsing---custom-attribute)
    - [Argument Reading (Stream only)](#argument-reading-stream-only)

## Basics
### Preparation:
```c#
using ArgParseSharp;

class Options {
    public bool MyFlag { get; set; }
    public string Input { get; set; }
    public int Index { get; set; }
}

// ...
// We will assume these args:
string[] args = new string[] {
    "-myflag",
    "-input", "asd",
    "-index", "42",
    "-myExtra", "extra"
};
```

### Automatic parsing:
```c#
// by omitting args parameter or passing null
// the CLI program arguments are used
Options options = ArgumentParser.AutoParse<Options>(args);

// USAGE EXAMPLE:
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

### Read non-recognized arguments:
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
// eg. from file
using FileStream fs = File.OpenRead("myfile.txt");
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

## Parsing directives
### Automatic inference:
```c#
// It is possible to automatically parse any class (Options in our case)
Options options = ArgumentParser.AutoParse<Options>(args);
// You can tag fields and properties with
// ArgumentAttributes to make it explicit to
// use a certain Parser for a member.
class Options {
    // Mark with arguments attribute a property
    // in order to instruct the argument parser
    // to discover these fields and properties
    [Argument.Int]
    public int IntField;
    [Argument.Boolean]
    public bool BoolProperty { get; set; }

    // Public non-readonly fields and properties
    // are automatically inferred if an auto-parser
    // type is already defined

    // automatically inferred property -> Argument.Boolean
    public bool AutoBoolProperty { get; set; }
    // automatically inferred field -> Argument.Boolean
    public bool AutoBoolField;

    // The attribute tells to the parser to
    // consider this property even if private
    [Argument.Boolean]
    private bool PrivateMarkedBoolProperty { get; set; }

    // The same applies for fields
    [Argument.Boolean]
    private bool PrivateMarkedBoolField;

    // Ignore a public field or property from automatic inference
    [Ignore]
    public bool IgnoredBoolField;
}

// Define an auto-parser
ArgumentParser.SetAutoParser<Argument.Boolean, bool>();
// OR
ArgumentParser.SetAutoParser(typeof(Argument.Boolean), typeof(bool));
```
### Disable automatic inference:
```c#
// You can put Ignore attribute on a class to
// disable the automatic inference
// (keep only marked field and properties)
[Ignore]
class Options {
    public bool IgnoredBoolField;
    public bool IgnoredBoolProperty { get; set; }
    // this will be the only argument being discovered
    [Argument.Boolean]
    public bool MarkedBoolProperty { get; set; }
}

// It is also possible to disable automatic inference with an argument
Options options = ArgumentParser.AutoParse<Options>(false, args); // disabled
Options options = ArgumentParser.AutoParse<Options>(true, args); // enabled
```
### Useful properties for arguments and casting:
```c#
class Options {
    // Use ArgumentAttribute Properties to
    // better describe the behaviour of an
    // auto-discovered argument
    [Argument.Boolean(
        Name = "primary-name",
        Flags = new stirng[]{ "-pn" },
        Help = "This is a primary name",
        Required = true
    )]
    // false = 0, true = 1 
    private int AutoCastedBoolField;

    // Other utilities

    // unlimited array of ints
    [Argument.Int(ArgNumber = '*')]
    public int[] ArrayIntProperty { get; private set; }

    // int array with specified size
    [Argument.Int(ArgNumber = 5, MetaVariables = new string[] {
        // variable names used in help (optional)
        //produces [ArrayIntField5 A B C D E]
        "A", "B", "C", "D", "E"
    })]
    public int[] ArrayIntField5;

    // Inverted boolean flag
    [Argument.Boolean(Constant = false, Default = true)]
    public bool InvertedFlagField;

    [Argument.File(
        // ignores prefix if present when parsed
        // eg: @myfile.txt will be parsed as myfile.txt
        IgnoredPrefix = "@"
    )]
    public FileInfo MyFile;

    // automatic version parser
    [Argument.VersionValue]
    public Version MyVersion;

    // automatic type inference
    [Argument.Type]
    public Type MyType;
}
```

## Customization
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
// (eg. automatically infer the Parser delegate: T ParserDelegate(string))
class MyArgument : Argument<MyArgumentValue> {
    // constructor with custom parser (and automatic default)
    public MyArgument(string name, ParserDelegate? parser = null) :
        base(name, parser == null ? DefaultParser : parser) { }
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
    Inherited = true
)]
class MyAttribute : ArgumentAttribute {
    // The type the parser should return
    protected override Type _resultType { get; } = typeof(MyArgumentValue);
    // The delegate type of the parser
    protected override Type _delegateType { get; } = typeof(MyArgument.ParserDelegate);
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
        return new MyArgument(name, (MyArgument.ParserDelegate?)parser);
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

// and for automatic inference
ArgumentParser.SetAutoParser<MyArgument, MyArgumentValue>();
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