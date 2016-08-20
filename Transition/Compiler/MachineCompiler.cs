namespace Transition.Compiler
{
   /// <summary>
   /// Compiler will lex, parse, analyze, and assemble a string of input into an executable Machine.
   /// </summary>
   public class MachineCompiler<T> where T : Context
   {
      private readonly Scanner _scanner;
      private readonly Parser _parser;
      private readonly SymanticAnalyzer _analyzer;
      private readonly MachineGenerator<T> _generator;

      public MachineCompiler()
      {
         _scanner = new Scanner();
         _parser = new Parser();
         _analyzer = new SymanticAnalyzer();
         _generator = new MachineGenerator<T>();
      }

      /// <summary>
      /// Loads Actions that will be used future compiled machines
      /// </summary>
      public void LoadActions(params System.Type[] actions)
      {
         _generator.LoadActions(actions);
      }

      /// <summary>
      /// Loads ValueConverters that will be used future compiled machines
      /// </summary>
      public void LoadValueConverters(params System.Type[] valueConverters)
      {
         _generator.LoadValueConverters(valueConverters);
      }

      /// <summary>
      /// Converts a string of input into an executable machine
      /// </summary>
      public Machine<T> Compile(string input)
      {
         ErrorCode errorCode;
         var charArray = input.ToCharArray();
         var tokens = _scanner.Scan(charArray, input.Length);
         if (!_scanner.DidReachEndOfInput()) {
            throw new System.Exception(string.Format("Error found {0}<--",_scanner.GetErrorLocation(input, 10)));
         }
         var rootNode = _parser.Parse(tokens, input);
         _analyzer.Analyze(rootNode, out errorCode);
         var machine = _generator.Generate(rootNode);
         return machine;
      }
   }
}
