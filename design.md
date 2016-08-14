# A design document for the language and it's components

## components
* Compiler
  * **Scanner** - Turns a string of input into a sequence of tokens
  * **Parser** - Creates an Abstract Syntax Tree from a sequence of tokens
  * **SymanticAnalyzer** - Analyzes an Abstract Syntax Tree for symantic errors
  * **MachineGenerator** - Synthesizes an executable Machine from an Abstract Syntax Tree
  * **MachineCompiler** - Wraps the Scanner, Parser, SymanticValidator and MachineGenerator in a simple abstraction
* **MachineController** - ticks a machine, handles messaging
* **Context** - the state for a machine instance for an actor
* **Blackboard** - A shared bag of state
* **ValueConverters**
  * bool
  * string
  * float
  * int
* **Nodes**
  * Machine
  * State
  * Action
    * TransitionAction
  * MessageEnvelope

# Execution Design

## Ticking Semantics
Should we run everything as fast as possible?

Idea 1: Spread load out by not allowing multiple transitions in a single tick.
- eg:
  - tick 1: A transition and enter
  - tick 2: Start running
- pros:
  - clear load semantics
- cons:
  - complexity. What happens when an actor is sent a message and transitions but is then ran later in the same tick?

Idea 2: Run as fast as possible. Add a yield action to allow the user to make their own semantics
- eg:
  - tick 1: tx -> enter -> run -> tx -> enter ->run
- pros:
  - simple to implement
  - simple to understand
- cons:
  - could lead to bad performance until you wrap your head around it
  - lots of yield boilerplate everywhere

Decision: Start with **Idea 2** and measure performance

### Actions
* Can be stateful (eg a cooldown may countdown)
* Can return Running, Done or Transition
* Can receive Named Parameters
* Can receive Named transitions
* Can receive default transition
* can receive default parameter
* does not have to transition
* Each action instance has a unique id within a state

### Messages
* Must be followed by a single action

### Machine
* requires a default start state
* (future) may be nestable
* Has a unique id

### State
* May have Enter, Exit, Run actions
* May have messages
* Each state has a uniqueId within a machine
* Executes actions from top to bottom in this order:
  * enter
  * run
  * exit
* Has different semantics for sections
  * enter, exit
    * All actions must return Done. Thus all actions will execute in the same tick.
  * run
    * Actions will be run sequentially and can take as many ticks as they wish
    * Actions can transition
  * on
    * Actions must return done or transition. No long running actions

### Param
* May have a name or the
* Must have a value
* Has an operator ':' or '->' which correspond to 'values' and 'transitions'.
  Transitions are special values.

### Special actions
* go (eg. -> 'state') is an action without a name that just transitions to a state.
* yield: returns running on one frame and done on the next. Can take a number parameter to delay for that many ticks

## Messaging Semantics

There are a couple ways of handling messaging between Machines and from outside systems into a Machine.

1. Sending just enqueues a message. The message queue can be emptied immediately, after an action returns a result or after all Machine's have been run.
2. Send all messages right away.

Going with the first option provides many opportunities to control performance and "fairness" of message deliverability. The second option leaves it up to the user to tune performance and couples performance to their design. However, the semantics are much simpler to comprehend. Despite the complexity I believe the first option is the correct way to go as it offers plenty of flexibility to optimize for performance, fairness or simplicity.

With that decided a question remains: When should we empty the queue? There are two dimensions to this question: Machine-to-Machine and Outside-Component-to-Machine communication. The discussion below explains the details but the conclusion will be defined here:

### Messaging Semantics Conclusion
1. Messages sent by outside components will be queued and processed at the start of the next tick before any Machine is ticked.
2. Messages sent during a tick will be queued and sent after all Machines have been ticked to ensure execution fairness.

### Outside Component to Machine messaging
Imagine a situation where a player in a game invokes casts a spell via a hotkey. Internally an input manager sends a message to a Machine called 'castspell'. What happens and when?

#### Case 1
1. The message goes into a Queue
2. Nothing happens until the next Tick. On the next Tick all messages in the queue are answered and then the Machines are ticked.

This ensures that a Machine's state is only ever updated during the Tick process and provides a level of determinism to the overall semantics of the entire Tick process that may make debugging easier. This is especially useful if you are creating a simulation who's results should be reproducible. Conversely there is a bit of wasted processing time.

#### Case 2
1. The message goes into a Queue
2. The queue is emptied immediately (up to a certain amount depending on performance and fairness tuning).

In this way no processing time is wasted. However this provides an opportunity for a Machine to change state many times before the next tick. If your game or simulation relies on a specific order of operations (eg. check input > update scores > Tick > spawn resources) bugs may be difficult to track down.

#### Outside Component to Machine Conclusion
It seems that sending messages at the start of the next tick is a good initial direction to try. With increased Determinism there comes a form of simplicity that is worth aiming for.

### Machine to Machine messaging
Machine to Machine messaging has an interesting order-of-operations fairness problem embedded in it. If Machines are Ticked int he order they are spawned then the first has an opportunity to change the second Machine's state before the other has a chance. This would happen if messages are sent right as they are fired. However if they are sen t at the end of the tick (after all Machines have run), each Machine has an equal chance to execute and the user of this library no longer has to struggle with an order-of-operations fairness problem.

For this reason we will start with this semantic.
