namespace Transition
{
   /// <summary>
   /// An Action or State returns a TickResult with an embeded TickResultType
   /// to indicate how a State or Machine should proceede. See <term>State</term> for more details.
   /// </summary>
   public enum TickResultType
   {
      Yield,
      Transition,
      Done,
      Loop
   }
}
