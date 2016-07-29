# transition

A light-weight state machine language, parser and interpreter built for run-time compilation and execution. 

<pre class="code">
<strong>@machine</strong> Worker <strong>-&gt;</strong> <em>'Idle'</em>

<strong>@state</strong> Idle
  <strong>@enter</strong>
    setAnim 'idle'
  <strong>@on</strong>
    'HarvestResource': <strong>-&gt;</strong> <em>'SeekResource'</em>

<strong>@state</strong> SeekResource
  <strong>@enter</strong>
    setAnim 'moving'
    setTarget 'Resource'
  <strong>@run</strong>
    seekTarget 'Resource'
    <strong>-&gt;</strong> <em>'HarvestResource'</em>

<strong>@state</strong> HarvestResource
  <strong>@enter</strong>
    claimResource
    setAnim 'harvesting'
  <strong>@run</strong>
    harvest speed:'3.0'
    <strong>-&gt;</strong> <em>'ReturnResources'</em>
  <strong>@on</strong>
    'cancelJob': canDropResources? threshold:'3' yes<strong>-&gt;</strong><em>'Idle'</em>

<strong>@state</strong> ReturnResources
  <strong>@enter</strong>
    setAnim 'moving'
    setTarget 'Base'
  <strong>@run</strong>
    seekTarget 'Base'
    depositResources
    <strong>-&gt;</strong> <em>'Idle'</em>
  <strong>@exit</strong>
    log format:'returnedResources %s' param:'resourceAmt'
</pre>

### Want to contribute?

See design.md for details, guidelines and philosophies. Feel free to open issues for feature requests.
