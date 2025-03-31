using _UTIL_;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _COBALT_
{
    partial class Command
    {
        public class Executor : IDisposable
        {
            public readonly ListListener<Executor> stack;
            public readonly Command command;

            static byte id_counter;
            public byte id = ++id_counter;

            public CommandLine commandline;
            public readonly IEnumerator<CMD_STATUS> routine;
            public CMD_STATUS status;

            public readonly ThreadSafe_struct<bool> disposed = new();

            //--------------------------------------------------------------------------------------------------------------

            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
            static void OnBeforeSceneLoad() => id_counter = 0;

            //--------------------------------------------------------------------------------------------------------------

            public Executor(in ListListener<Executor> stack, in Command command)
            {
                this.stack = stack;
                this.command = command;

                if (command.routine != null)
                {
                    routine = command.routine(this);
                    routine.Reset();
                }
            }

            //--------------------------------------------------------------------------------------------------------------

            public CMD_STATUS Executate(in CommandLine line)
            {
                commandline = line;
                if (command.commands.Count == 0)
                    if (command.action != null)
                        status = command.action(line);
                    else if (routine.MoveNext())
                        status = routine.Current;
                    else if (line.TryReadArgument(out string argument))
                        if (this.command.commands.TryGetValue(argument, out Command command))
                        {
                            Executor executor = new(stack, command);
                            if (line.signal == CMD_SIGNAL.EXEC)
                                stack.AddElement(executor);
                            status = executor.Executate(line);
                        }
                return status;
            }

            //--------------------------------------------------------------------------------------------------------------

            public void Dispose()
            {
                lock (disposed)
                {
                    if (disposed._value)
                        return;
                    disposed._value = true;
                    stack.RemoveElement(this);
                    OnDispose();
                }
            }

            protected virtual void OnDispose()
            {
            }
        }
    }
}