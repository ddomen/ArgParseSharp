namespace ArgParseSharp;

public partial class Argument {
    public class Float : Argument<float> {
        public Float(string name, float constant) : base(name, constant) { }
        public Float(string name, ParserDelegate? parser = null) : base(name, parser ?? DefaultParser) { }
        public static float DefaultParser(string value) => float.TryParse(value, out float r) ? r : 0f;
    }
}
