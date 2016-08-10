using System.Reflection;

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
      /// Call before Compile to build a lookup table of Actions to instantiate during generation.
      /// </summary>
      public void Initialize(params Assembly[] assemblies)
      {
         _generator.Initialize(assemblies);
      }

      /// <summary>
      /// Converts a string of input into an executable machine
      /// </summary>
      public Machine<T> Compile(string input)
      {
         ErrorCode errorCode;
         var charArray = input.ToCharArray();
         var tokens = _scanner.Scan(charArray, input.Length);
         var rootNode = _parser.Parse(tokens, input);
         _analyzer.Analyze(rootNode, out errorCode);
         var machine = _generator.Generate(rootNode);
         return machine;
      }
   }
}
