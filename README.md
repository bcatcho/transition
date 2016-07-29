# transition

A light weight c# state machine lang, parser and interpreter built for run-time compilation and execution.

<pre class="code"><span class="pl-k">@machine</span> <span class="pl-en">Worker</span> <span class="pl-k">-&gt;</span> <span class="pl-c1">&#39;Idle&#39;</span>

<span class="pl-k">@state</span> <span class="pl-en">Idle</span>
  <span class="pl-s3">@enter</span>
    setAnim <span class="pl-s1"><span class="pl-pds">&#39;</span>idle<span class="pl-pds">&#39;</span></span>
  <span class="pl-s3">@on</span>
    <span class="pl-s1">&#39;HarvestResource&#39;</span> <span class="pl-k">-&gt;</span> <span class="pl-c1">&#39;SeekResource&#39;</span>

<span class="pl-k">@state</span> <span class="pl-en">SeekResource</span>
  <span class="pl-s3">@enter</span>
    setAnim <span class="pl-s1"><span class="pl-pds">&#39;</span>moving<span class="pl-pds">&#39;</span></span>
    setTarget <span class="pl-s1"><span class="pl-pds">&#39;</span>Resource<span class="pl-pds">&#39;</span></span>
  <span class="pl-s3">@run</span>
    seekTarget <span class="pl-s1"><span class="pl-pds">&#39;</span>Resource<span class="pl-pds">&#39;</span></span>
    <span class="pl-k">-&gt;</span> <span class="pl-c1">&#39;HarvestResource&#39;</span>

<span class="pl-k">@state</span> <span class="pl-en">HarvestResource</span>
  <span class="pl-s3">@enter</span>
    claimResource
    setAnim <span class="pl-s1"><span class="pl-pds">&#39;</span>harvesting<span class="pl-pds">&#39;</span></span>
  <span class="pl-s3">@run</span>
    harvest speed<span class="pl-k">:</span><span class="pl-s1"><span class="pl-pds">&#39;</span>3.0<span class="pl-pds">&#39;</span></span>
    <span class="pl-k">-&gt;</span> <span class="pl-c1">&#39;ReturnResources&#39;</span>
  <span class="pl-s3">@on</span>
    <span class="pl-s1">&#39;cancelJob&#39;</span><span class="pl-k">:</span> canDropResources<span class="pl-k">?</span> threshold<span class="pl-k">:</span><span class="pl-s1"><span class="pl-pds">&#39;</span>3<span class="pl-pds">&#39;</span></span> yes<span class="pl-k">-&gt;</span><span class="pl-c1">&#39;Idle&#39;</span>

<span class="pl-k">@state</span> <span class="pl-en">ReturnResources</span>
  <span class="pl-s3">@enter</span>
    setAnim <span class="pl-s1"><span class="pl-pds">&#39;</span>moving<span class="pl-pds">&#39;</span></span>
    setTarget <span class="pl-s1"><span class="pl-pds">&#39;</span>Base<span class="pl-pds">&#39;</span></span>
  <span class="pl-s3">@run</span>
    seekTarget <span class="pl-s1"><span class="pl-pds">&#39;</span>Base<span class="pl-pds">&#39;</span></span>
    depositResources
    <span class="pl-k">-&gt;</span> <span class="pl-c1">&#39;Idle&#39;</span>
</pre>

# Want to contribute?

See design.md for details, guidelines and philosophies. Feel free to open issues for feature requests.
