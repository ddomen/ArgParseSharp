namespace ArgParseSharp;

public partial class Argument {
    public class Double : Argument<double> {
        public Double(string name, double constant) : base(name, constant) { }
        public Double(string name, Parser? parser = null) : base(name, parser ?? DefaultParser) { }
        public static double DefaultParser(string value) => double.TryParse(value, out double r) ? r : 0.0;
    }
}
