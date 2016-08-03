using System.Collections.Generic;

namespace Transition
{

   public enum ErrorCode
   {
      None,

      /// <summary>
      /// An action must return yield inside of the Enter section of a state
      /// </summary>
      State_Enter_ActionDidNotReturnYield
   }
}
