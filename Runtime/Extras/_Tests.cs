using _COBRA_;

namespace _COBALT_
{
    partial class CmdUI
    {
        static void Init_Tests()
        {
            Command extras = Command.static_domain.AddDomain(new("useful"), aliases: "useless");

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