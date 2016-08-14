# transition

A light-weight state machine language, parser and interpreter built for run-time compilation and execution. It's designed to create simple state machine's out of reusable components instead of custom coded states. In this way it's similar to behavior trees, but only in that way.


<a href="https://travis-ci.org/bcatcho/transition"><img src="https://travis-ci.org/bcatcho/transition.svg?branch=master" alt="Build Status"></a>
<a href="https://twitter.com/catchco"><img src="https://img.shields.io/badge/twitter-follow%20%40catchco-blue.svg" alt="Twitter Follow Me"></a>


- [A simple example for a simple language](#a-simple-example-for-a-simple-language)
	- [The Machine and Code](#the-machine-and-code)
	- [Machine Definition Line](#machine-definition-line)
	- [Closed State](#closed-state)
	- [Open State](#open-state)
	- [SetAnimation Action](#setanimation-action)
	- [WaitForAnimation Action](#waitforanimation-action)
	- [Wait Action](#wait-action)
	- [Conclusion](#conclusion)
- [Contributing](#contributing)

## A simple example for a simple language
Let's demonstrate some important language features with a an example of a Machine (state machine) for a door that closes after three seconds. The examples pretend you are working in a _Unity-like_ environment. We will start from the product:

### The Machine and Code
**Here is an example machine**
<pre class="code">
<strong>@machine</strong> SelfClosingDoor -><em>'Closed'</em>

<strong>@state</strong> Closed
  <strong>@enter</strong>
    SetAnimation 'closed'
  <strong>@on</strong>
    'OpenDoor': ->'Open'

<strong>@state</strong> Open
  <strong>@enter</strong>
    SetAnimation 'open'
  <strong>@run</strong>
    WaitForAnimation
    Wait seconds:'3'
    -> <em>'Closed'</em>
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
<strong>@machine</strong> SelfClosingDoor -><em>'Closed'</em>
</pre>
This line is very straight forward and has three components: `@machine` keyword, a Name for the machine and which state to transition the first time it is run.

### Closed State
<pre class="code">
1 <strong>@state</strong> Closed
2   <strong>@enter</strong>
3     SetAnimation 'closed'
4   <strong>@on</strong>
5     'OpenDoor': ->'Open'
</pre>

This State is more interesting. Let's break it down line by line.

|line| description|
|---|---|
|1|The first line declares a new state and it's name.|
|2|On the second line we start a new _section_, in particular the `@enter` section. Any _Actions_ in this section will be run when the state is first entered.|
|3|The next line contains an _Action_. This is the bit that you would write code for. In this case it is an Action that sets some animation state.|
|4|Now we have reached a new section that is slightly special. An `@on` section contains a list of Actions that will be run in response to an incoming message. In this case the action is simply to transition to the 'Open' state.|
|5|Finally we have a _message-action tuple_ that instructs the State to transition to the `Open` State when the `OpenDoor` message is received if the door was unlocked. Any Action can be used in response to a message, not just Actions that transition. In fact this Action doesn't have to transition (which we will see later).|

### Open State
<pre class="code">
1 <strong>@state</strong> Open
2   <strong>@enter</strong>
3     SetAnimation 'open'
4   <strong>@run</strong>
5     WaitForAnimation
6     Wait seconds:'3'
7     -> <em>'Closed'</em>
</pre>

This state demonstrates even more language features. We will skip over those already mentioned above.

|line| description|
|---|---|
|3|This line is only interesting because it demonstrates _Action reuse_. Even though the Action contains a different parameter value, the code to make it work only needs to be written once.|
|4|The `@run` section is executed every frame in order. Actions in the `Run` section can take as many frames as is necessary to complete.|
|5|The `WaitForAnimation` Action will only exit once the door opening animation is complete. Thus, it may take a few frames before we reach line 6|
|6|The `Wait` Action takes a parameter that indicates how many seconds to wait before moving to the next Action.|
|7|The `-> 'closed'` is actually syntactic sugar for `$trans -> 'closed'`. It's a special built in action that makes transitioning to another state really easy to type.|

What follows is all the code necessary to power the Machine. While there is a little more code necessary to make it work in your game or simulation, it is the least interesting part.

### SetAnimation Action

The `SetAnimation` action is a great example of _Action reuse_. It is a small action that plays an animation and exits. The `OnTick` method is what is called by the Machine Controller when it run's your code. It uses the `TickResult` to determine if it should run the next action in the state or not. We will see examples of that later.

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
