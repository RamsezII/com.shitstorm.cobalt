using _UTIL_;

namespace _COBALT_
{
    partial class ScriptView
    {
        enum States : byte
        {
            NoFile,
            NoChanges,
            Changes,
        }

        readonly ValueNotifier<States> state = new();

        //--------------------------------------------------------------------------------------------------------------

        void InitState()
        {
            state.AddListener(value =>
            {
                switch (value)
                {
                    case States.NoFile:
                        
                        break;

                    case States.NoChanges:
                        break;

                    case States.Changes:
                        break;
                }
            });
        }
    }
}