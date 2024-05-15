using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Events
{
    public static readonly Evt<MobKilledEventArgs> onExampleEvent = new Evt<MobKilledEventArgs>();
    public static readonly Evt<string> onUniqueItemGeneratedOnSv = new Evt<string>();
}