using System.Collections.Generic;
using Transition.Compiler.AstNodes;

namespace Transition.Compiler
{
   /// <summary>
   /// Performs symantic analysis on a Machine's syntax tree and produces helpful error codes.
   /// </summary>
   public class SymanticAnalyzer
   {
      private Dictionary<string, int> _statesIds;

      public bool Analyze(MachineAstNode machineNode, out ErrorCode errorCode)
      {
         BuildStateIds(machineNode);
         if (!HydrateTransitionParameters(machineNode, out errorCode)) {
            return false;
         }

         return true;
      }

      private void BuildStateIds(MachineAstNode machineNode)
      {
         if (_statesIds == null) {
            _statesIds = new Dictionary<string, int>();
         }
         _statesIds.Clear();

         for (int i = 0; i < machineNode.States.Count; ++i) {
            _statesIds.Add(machineNode.States[i].IdentifierLower, i);
         }
      }

      /// <summary>
      /// Check all transition parameters to see if they indentify a valid state and set their "StateIdVal" property
      /// to be the state they are trying to transition to.
      /// </summary>
      /// <returns><c>true</c>, if all transitions are valid, <c>false</c> otherwise.</returns>
      /// <param name="machine">Machine to validate.</param>
      /// <param name="errorCode">Error code if any.</param>
      private bool HydrateTransitionParameters(MachineAstNode machineNode, out ErrorCode errorCode)
      {
         errorCode = ErrorCode.None;

         if (!HydrateTransitionParametersInSection(machineNode.On, out errorCode))
            return false;
         
         StateAstNode state;
         for (int i = 0; i < machineNode.States.Count; ++i) {
            state = machineNode.States[i];
            if (!HydrateTransitionParametersInSection(state.Enter, out errorCode)
                || !HydrateTransitionParametersInSection(state.Exit, out errorCode)
                || !HydrateTransitionParametersInSection(state.Run, out errorCode)
                || !HydrateTransitionParametersInSection(state.On, out errorCode)) {
               return false;
            }
         }

         return true;
      }

      private bool HydrateTransitionParametersInSection(SectionAstNode sectionNode, out ErrorCode errorCode)
      {
         ParamAstNode param;
         string paramVal;
         errorCode = ErrorCode.None;
         if (sectionNode != null) {
            for (int j = 0; j < sectionNode.Actions.Count; ++j) {
               for (int p = 0; p < sectionNode.Actions[j].Params.Count; ++p) {
                  param = sectionNode.Actions[j].Params[p];
                  if (param.Op == ParamOperation.Transition) {
                     paramVal = param.Val.ToLower();
                     if (!_statesIds.ContainsKey(paramVal)) {
                        errorCode = ErrorCode.Validate_TransitionParams_StateNotFoundForTransition;
                        return false;
                     }
                     param.StateIdVal = _statesIds[paramVal];
                  }
               }
            }
         }
         return true;
      }
   }
}
