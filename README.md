# transition

A light-weight state machine language, parser and interpreter built for run-time compilation and execution. It's designed to create simple state machine's out of reusable components instead of custom coded states.

Though Transition was designed for Unity development it has _no_ dependencies other than `System` (and a few child namespaces) and will work for any C# based project.

This library is fully compatible with iOS/Android Unity development.

<a href="https://travis-ci.org/bcatcho/transition"><img src="https://travis-ci.org/bcatcho/transition.svg?branch=master" alt="Build Status"></a>
<a href="https://twitter.com/catchco"><img src="https://img.shields.io/badge/twitter-follow%20%40catchco-blue.svg" alt="Twitter Follow Me"></a>

Describe and execute state machines like this:
<pre class="code">
<strong>@machine</strong> SelfClosingDoor -><em>Closed</em>

<strong>@state</strong> Closed
  <strong>@enter</strong>
    SetAnimation 'closed'
  <strong>@on</strong>
    'OpenDoor': -><em>Open</em>

<strong>@state</strong> Open
  <strong>@enter</strong>
    SetAnimation 'open'
  <strong>@run</strong>
    WaitForAnimation
    Wait seconds:'3'
    -><em>Closed</em>
</pre>


## Start Here
### [Get the Latest Release](https://github.com/bcatcho/transition/releases)
### [Unity Transition Examples](https://github.com/bcatcho/transition-unity-examples)
An example project in Unity.
### [Tutorial: Setting up in Unity](https://github.com/bcatcho/transition/wiki/Tutorial:-Setting-up-in-Unity)
A guide for all the basic components needed to get up and running in Unity.
### Watch an introduction video
[![Introduction Video](http://img.youtube.com/vi/HBU4DjntMxQ/hqdefault.jpg)](https://www.youtube.com/watch?v=HBU4DjntMxQ)


---
<!-- TOC depthFrom:2 depthTo:6 withLinks:1 updateOnSave:1 orderedList:0 -->

- [Start Here](#start-here)
	- [[Get the Latest Release](https://github.com/bcatcho/transition/releases)](#get-the-latest-releasehttpsgithubcombcatchotransitionreleases)
	- [[Unity Transition Examples](https://github.com/bcatcho/transition-unity-examples)](#unity-transition-exampleshttpsgithubcombcatchotransition-unity-examples)
	- [[Tutorial: Setting up in Unity](https://github.com/bcatcho/transition/wiki/Tutorial:-Setting-up-in-Unity)](#tutorial-setting-up-in-unityhttpsgithubcombcatchotransitionwikitutorial-setting-up-in-unity)
	- [Watch an introduction video](#watch-an-introduction-video)
- [Language concepts](#language-concepts)
	- [@machine](#machine)
	- [@state](#state)
	- [@run](#run)
	- [@enter, @exit](#enter-exit)
	- [@on](#on)
	- [Actions](#actions)
		- [Return Types (controlling execution flow)](#return-types-controlling-execution-flow)
		- [Parameters](#parameters)
		- [Transition Parameters](#transition-parameters)
	- [Comments](#comments)
- [Ticking](#ticking)
	- [Tick boundaries (timing)](#tick-boundaries-timing)
		- [Transitions](#transitions)
		- [First Tick](#first-tick)
		- [Return Type: Yield](#return-type-yield)
		- [Return Type: Done](#return-type-done)
		- [Return Type: Loop](#return-type-loop)
- [Messaging](#messaging)
- [MachineController](#machinecontroller)
- [Value Converters](#value-converters)
- [Contexts](#contexts)
- [Default Parameters](#default-parameters)
- [AltId](#altid)
- [Blackboard](#blackboard)
	- [ValueWrapper](#valuewrapper)
- [An example in excruciating detail](#an-example-in-excruciating-detail)
	- [The Machine and Code](#the-machine-and-code)
	- [Machine Definition Line](#machine-definition-line)
	- [Closed State](#closed-state)
	- [Open State](#open-state)
	- [SetAnimation Action](#setanimation-action)
	- [WaitForAnimation Action](#waitforanimation-action)
	- [Wait Action](#wait-action)
	- [Conclusion](#conclusion)
- [Contributing](#contributing)

<!-- /TOC -->

## Language concepts
A Machine (state machine) in Transition looks like this:

```
@machine MachineName ->FirstState
  @on
    'sing': say 'lalalala'

@state FirstState
  @enter
    DoSomethingOnEnter
  @run
    say 'hi'
    moveRight speed:'3' distance:'2'
    ->StateTwo
  @exit
    say message:'bye'
  @on
    'poke': say 'ouch!'
```

### @machine
```
@machine Name -> StateName
```
The first line in a machine describes the name of the machine and the first state
to transition toward. This line is required in all machines.

A Machine can have global message handlers. These are placed in an optional `@on` section beneath the
`@machine` line. See the [@on](#on) section for more details.


### @state

```
@state Name
```
A state has a name and 4 sections: `@enter`, `@run`, `@exit`, `@on`. A state can
exist without anything but this first line.

### @run
```
@state Name
  @run
    Action
    Action2
```

The `@run` section describes a list of Actions to run in order. Every time the
machine is _ticked_ the current state will run the active Action. If all Actions have
run the state will not perform any Actions on subsequent ticks. Actions are the primary
way of controlling behavior and are discussed below.

### @enter, @exit
```
@state Name
  @enter
    Action
    Action2
  @exit
    Action2
    Action3
```
The `@enter` and `@exit` state sections describe a list of Actions to run in order when
a state is entered or exited and only then.

*Actions in this section are special*. They _must_ return `TickResult.Done()`. In other words
they must end in the same tick they are run and can not transition to a different state. This
is discussed in the [Ticking](#ticking) section.

### @on
```
@state Name
  @on
    'message': Action
    'message2': Action
```
The `@on` section is a special state section that describes a collection of messages
and associated Actions. Each message can have only one Action. When a Machine is sent an
Action it will look in this section to see if the current state can handle it. If so the
associated Action will be run.

*Actions in this section are special*. They _must_ return `TickResult.Done()` or transition.
This is discussed in the [Messaging](#messaging) section.

### Actions
```
@state Name
  @run
    Action param:'3' transitionParam->OtherState
```
Actions are the part that makes everything work. They are associated with a c# class
that you must write. Actions can accept as many parameters as you wish including special
Transition Parameters.

#### Return Types (controlling execution flow)
When implementing an Action there are 4 return types to choose from. Each affects the
Machine differently and all control the flow of behavior in some way.

* **Done** specifies that the Action is done and that the next (if any) should be run. Use `TickResult.Done()` to create this type of result.

* **Yield** specifies that the Action is not done processing and should be run again on the
next Tick. This allows actions to execute over many Ticks (or in-game time). Useful for
behavior like movement, delays, animation. Use `TickResult.Yield()` to create this type of result.

* **Transition** tells the machine to transition to a state. To specify which state to
transition to you must use a `TransitionDestination`. This is described in the section
on [Transition Parameters](#transition-parameters). Use the protected method `TransitionTo()`
to return a `Transition` result.

* **Loop** tells the current state to run the first action in the section on the very next Tick.

Example actions:
```csharp
public class SimpleAction : Action<Context> {
  protected override TickResult OnTick(Context context) {
    // return done and move on to the next action
    return TickResult.Done();
  }
}
```
```csharp
public class LongRunningAction : Action<Context> {
  protected override TickResult OnTick(Context context) {
    // always returning yield means the next action will never be run
    return TickResult.Yield();
  }
}
```
```csharp
public class TransitionAction : Action<Context> {
  public TransitionDestination NextState {get; set;}

  protected override TickResult OnTick(Context context) {
    // TransitionTo is a built in method to help build a Transition TickResult
    return TransitionTo(NextState);
  }
}
```
```csharp
public class LoopingAction : Action<Context> {
  protected override TickResult OnTick(Context context) {
    // this will restart the state at the very first @run action in the next tick
    return TickResult.Loop();
  }
}
```
#### Parameters

An action can have many parameters. Each parameter is associated with a property in the
backing class. For instance an action that takes an c# `int` and `string` called `a` and `b` respectively
would look like this:

`exampleAction a:'200' b:'woof'`

```csharp
public class ExampleAction : Action<Context> {
  public int A {get; set;}
  public string B {get; set;}
  //... the rest of the action
}
```

The few types that are supported by default are: `float`, `int`, `string`. However, you can
support any type of object or value just by implementing a `IValueConverter`. See the section
on [Value Converters](#value-converters) for details.

#### Transition Parameters

If you want your state to transition to another (or the same) you'll need to use a special
type of parameter. Any state name that occurs after an arrow `->` will be assigned to that
parameter. The parameter must be backed by a property with the `TransitionDestination` type.
For example:

`exampleAction a->NextState`

```csharp
public class ExampleAction : Action<Context> {
  public TransitionDestination A {get; set;}
  //... the rest of the action
}
```

### Comments

Comments are made by using the `#` symbol. These are all valid comments:

```
#comment on it's own line
@state Name #comment at end of line
	@run
	  #comments inside of sections
```

## Ticking

"Ticking" is the process of running a Machine. When a Machine is ticked the current
state's current action is executed and based on the TickResult returned an action
is taken.

### Tick boundaries (timing)

Certain actions happen can happen in the same tick, but for performance and conceptual
reasons some can't.

#### Transitions
When a state transitions to another, the `@exit` and `@enter` sections are executed.
The `@run` section of the new state is not executed until the next tick:

*Tick 1*

1. An action in State A causes transition to State B
2. All Actions in State A's `@exit` section are executed in order
3. All Actions in State B's `@enter` section are executed in order

*Tick 2*

The first Action in State B's `@run` section is executed (and processing occurs normally).

This behavior is meant to reduce the need for an implementor to worry about the
performance of the Machine. Without this boundary it would be very easy to create
infinite loops between states.

#### First Tick
When a Machine is first ticked it must transition to the initial state. This
follows all the rules of the Transition Tick Boundary above. Thus, the first
Action in the Initial State's `@run` section won't be executed until the second tick.

#### Return Type: Yield
Yielding (see section on return types) tells the machine to stop processing and start at the same Action on the next tick. It is the simplest form of a tick boundary.

#### Return Type: Done
Returning `Done` from an action does not cause a tick boundary. The next Action in the same section will be run immediately.

#### Return Type: Loop
Returning `Loop` does cause a tick boundary. The first Action in the same section will be run on the _next_ tick. This is to help prevent infinite loops.

## Messaging
TODO

## MachineController
TODO
## Value Converters
TODO
## Contexts
TODO
## Default Parameters
TODO
## AltId
TODO
## Blackboard
TODO
### ValueWrapper
TODO
---

## An example in excruciating detail
Let's demonstrate some important language features with a an example of a Machine for a door that closes after three seconds. The examples pretend you are working in a _Unity-like_ environment. We will start from the product:

### The Machine and Code
**Here is an example machine**
<pre class="code">
<strong>@machine</strong> SelfClosingDoor -><em>Closed</em>

<strong>@state</strong> Closed
  <strong>@enter</strong>
    SetAnimation 'closed'
  <strong>@on</strong>
    'OpenDoor': -><em>Open</em>

<strong>@state</strong> Open
  <strong>@enter</strong>
    SetAnimation 'open'
  <strong>@run</strong>
    WaitForAnimation
    Wait seconds:'3'
    -> <em>Closed</em>
</pre>

**Here is all the code it takes to power it**
```csharp
public class SetAnimation : Action<UnityContext>
{
  [DefaultParameter]
  public string AnimationVal { get; set; }

  protected override TickResult OnTick (UnityContext context) {
    context.GameObject.GetComponent<Animator>().Play(AnimationVal);
    return TickResult.Done();
  }
}
```
```csharp
public class WaitForAnimation : Action<UnityContext>
{
  protected override TickResult OnTick (UnityContext context) {
    var animator = context.GameObject.GetComponent<Animator>();
    return animator.IsAnimating() ? TickResult.Yield() : TickResult.Done();
  }
}
```
```csharp
public class Wait : Action<UnityContext>
{
  public float Seconds { get; set; }

  protected override void OnEnterAction (UnityContext context) {
    context.BlackBoard.Set<float>("elapsedTime", 0);
  }

  protected override TickResult OnTick (UnityContext context) {
	  var elapsedTime = context.BlackBoard.Get<float>("elapsedTime");
	  elapsedTime += Time.deltaTime;
	  context.BlackBoard.Set<float>("elapsedTime", elapsedTime);
	  return elapsedTime > Seconds ? TickResult.Done() : TickResult.Yield();
	}
}
```

Now we can dissect it section by section, line by line.

### Machine Definition Line
<pre class="code">
<strong>@machine</strong> SelfClosingDoor -><em>Closed</em>
</pre>
This line is very straight forward and has three components: `@machine` keyword, a Name for the machine and which state to transition the first time it is run.

### Closed State
<pre class="code">
1 <strong>@state</strong> Closed
2   <strong>@enter</strong>
3     SetAnimation 'closed'
4   <strong>@on</strong>
5     'OpenDoor': ->Open
</pre>

This State is more interesting. Let's break it down line by line.

|line| description|
|---|---|
|1|The first line declares a new state and it's name.|
|2|On the second line we start a new _section_, in particular the `@enter` section. Any _Actions_ in this section will be run when the state is first entered.|
|3|The next line contains an _Action_. This is the bit that you would write code for. In this case it is an Action that sets some animation state.|
|4|Now we have reached a new section that is slightly special. An `@on` section contains a list of Actions that will be run in response to an incoming message. In this case the Action is simply to transition to the 'Open' state.|
|5|Finally we have a _message-Action tuple_ that instructs the State to transition to the `Open` State when the `OpenDoor` message is received if the door was unlocked. Any Action can be used in response to a message, not just Actions that transition. In fact this Action doesn't have to transition (which we will see later).|

### Open State
<pre class="code">
1 <strong>@state</strong> Open
2   <strong>@enter</strong>
3     SetAnimation 'open'
4   <strong>@run</strong>
5     WaitForAnimation
6     Wait seconds:'3'
7     -> <em>Closed</em>
</pre>

This state demonstrates even more language features. We will skip over those already mentioned above.

|line| description|
|---|---|
|3|This line is only interesting because it demonstrates _Action reuse_. Even though the Action contains a different parameter value, the code to make it work only needs to be written once.|
|4|The `@run` section is executed every frame in order. Actions in the `Run` section can take as many frames as is necessary to complete.|
|5|The `WaitForAnimation` Action will only exit once the door opening animation is complete. Thus, it may take a few frames before we reach line 6|
|6|The `Wait` Action takes a parameter that indicates how many seconds to wait before moving to the next Action.|
|7|The `-> closed` is actually syntactic sugar for `$trans -> closed`. It's a special built in Action that makes transitioning to another state really easy to type.|

What follows is all the code necessary to power the Machine. While there is a little more code necessary to make it work in your game or simulation, it is the least interesting part.

### SetAnimation Action

The `SetAnimation` Action is a great example of _Action reuse_. It is a small Action that plays an animation and exits. The `OnTick` method is what is called by the Machine Controller when it run's your code. It uses the `TickResult` to determine if it should run the next Action in the state or not. We will see examples of that later.

**Note** the `DefaultParameter` attribute - this allows for syntactic sugar: `SetAnimation AnimationVal:'blah'` is equivalent to `SetAnimation 'blah'`.

### WaitForAnimation Action

The `WaitForAnimation` Action checks the status of an animation and returns either `Yield` or `Done`. This is one of the interesting properties of Actions in the `@run` section - they can last many ticks.

### Wait Action

This was included in the example to demonstrate that Actions are run in sequence, but only after the previous has run. It also uses the BlackBoard (albeit in a very inefficient way) to store state. Actions are essentially stateless and use the Context to store state. This allows Machine's to be instantiated once and reused many times.

### Conclusion

As you can see, the Action concept promotes re-use of common tasks that happen in State Machine's. There are still some interesting concepts to explain such as the Machine Controller, MessageBus or transitioning but this will do for an initial demonstration.

---

## Contributing

It's very early but feel free to open issues and leave comments on any bugs or feature requests. See design.md for details, guidelines and philosophies.
