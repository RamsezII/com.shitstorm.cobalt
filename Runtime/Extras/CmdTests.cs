using _COBRA_;
using UnityEngine;

namespace _COBALT_
{
    static class CobaltExtras
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Command extras = Shell.static_domain.AddDomain(new("useful"), aliases: "useless");

            extras.AddAction(
                "deez",
                manual: new("HA!"),
                args: null,
                action: exe => exe.Stdout("nuts!")
                );

            extras.AddAction(
                "lezduit",
                manual: new("aherm.. whahh?"),
                args: null,
                action: exe => exe.Stdout("brah")
                );
        }
    }
}