# A design document for the language and it's components

## components
* Parser - creates syntax tree with metadata
* Compiler - instantiates syntax tree as an executable machine
* Executor - ticks a machine, handles messaging
* Context - the state for a machine instance for an actor
  * current state
  * current action
* ContextFactory - create custom contexts for your actions
* DefaultContextFactory
* Blackboard - bb
* Library - catalog of machines
* ValueConverters - used to parse param input
  * bool
  * string
  * float
  * int
* Nodes
  * machine
  * state
  * actions
    * action
    * transition
  * message

## syntax tree nodes
```
machine
  start
  state []

state
  enter
    action []
  run
    action []
  exit
    action []
  on
    message []

action
  name
  params
  transitions

param
  name
  value

message
  name
  action
```

# design

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
