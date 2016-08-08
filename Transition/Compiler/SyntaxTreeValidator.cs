using System.Collections.Generic;
using Transition.Compiler.AstNode;

namespace Transition.Compiler
{
   public class SyntaxTreeValidator
   {
      private HashSet<string> _statesIds;

      public bool TransitionsAreValid(MachineAstNode machine, out ErrorCode errorCode)
      {
         errorCode = ErrorCode.None;
         if (_statesIds == null) {
            _statesIds = new HashSet<string>();
         }
         _statesIds.Clear();

         for (int i = 0; i < machine.States.Count; ++i) {
            _statesIds.Add(machine.States[i].IdentifierLower);
         }

         StateAstNode state;
         for (int i = 0; i < machine.States.Count; ++i) {
            state = machine.States[i];
            if (!TransitionsInSectionAreValid(state.Enter, out errorCode)
                || !TransitionsInSectionAreValid(state.Exit, out errorCode)
                || !TransitionsInSectionAreValid(state.Run, out errorCode)
                || !TransitionsInSectionAreValid(state.On, out errorCode)) {
               return false;
            }
         }

         return true;
      }

      private bool TransitionsInSectionAreValid(SectionAstNode section, out ErrorCode errorCode)
      {
         ParamAstNode param;
         errorCode = ErrorCode.None;
         if (section != null) {
            for (int j = 0; j < section.Actions.Count; ++j) {
               for (int p = 0; p < section.Actions[j].Params.Count; ++p) {
                  param = section.Actions[j].Params[p];
                  if (param.Op == ParamOperation.Transition && !_statesIds.Contains(param.Val.ToLower())) {
                     errorCode = ErrorCode.Validate_TransitionParams_StateNotFoundForTransition;
                     return false;
                  }
               }
            }
         }
         return true;
      }
   }
}
