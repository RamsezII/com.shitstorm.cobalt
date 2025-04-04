using _UTIL_;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace _COBALT_
{
    partial class Command
    {
        public partial class Executor : IDisposable
        {
            //prefixe = $"{MachineSettings.machine_name.Value.SetColor("#73CC26")}:{NUCLEOR.terminal_path.SetColor("#73B2D9")}$",

            static readonly Executor echo_executor = new(new() { new("echo", new(on_stdin: (exe, stdin) => Debug.Log(stdin))), }, Line.EMPTY_EXE, out _);

            public readonly string cmd_name;
            public readonly Command command;
            public readonly string cmd_path;
            public readonly List<KeyValuePair<string, Command>> path;

            public Line temp_line;
            readonly Executor stdout_exe = echo_executor;
            public readonly List<object> args;
            public IEnumerator<CMD_STATUS> routine;

            public readonly ThreadSafe_struct<bool> disposed = new();

            public int executions = -1;
            static byte id_counter;
            public byte id = ++id_counter;

            public object error;
            public void Stdout(in string data) => stdout_exe.command.on_stdin(stdout_exe, data);

            //--------------------------------------------------------------------------------------------------------------

            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
            static void OnBeforeSceneLoad()
            {
                id_counter = 0;
            }

            //--------------------------------------------------------------------------------------------------------------

            public Executor(in List<KeyValuePair<string, Command>> path, in Line line, out bool error)
            {
                this.path = path;
                cmd_name = path[^1].Key;
                command = path[^1].Value;

                switch (path.Count)
                {
                    case 0:
                        throw new ArgumentException($"Command path is empty.", nameof(path));

                    case 1 when path[0].Value == cmd_root_shell:
                        cmd_path = "~";
                        break;

                    case 1:
                        cmd_path = path[0].Key;
                        break;

                    default:
                        StringBuilder path_sb = new();
                        for (int i = 0; i < path.Count; i++)
                            path_sb.Append(path[i].Key + "/");
                        path_sb.Remove(path_sb.Length - 1, 1);
                        cmd_path = path_sb.ToString();
                        break;
                }

                if (command.args != null)
                {
                    args = new();
                    command.args(this, line);
                    if (this.error != null)
                    {
                        if (line.signal == CMD_SIGNALS.EXEC || line.start_i > line.cpl_start_i)
                            Debug.LogWarning($"Command '{cmd_name}' failed to parse arguments.");
                        Dispose();
                        error = true;
                        return;
                    }
                }

                if (line.TryReadPipe())
                    if (cmd_root_shell.TryReadCommand(line, out var path2))
                    {
                        stdout_exe = new(path2, line, out bool err);
                        if (err)
                        {
                            Dispose();
                            error = true;
                            return;
                        }
                    }

                error = false;
            }

            //--------------------------------------------------------------------------------------------------------------

            public IEnumerator<CMD_STATUS> Executate(in Line line)
            {
                if (line.signal == CMD_SIGNALS.EXEC)
                    ++executions;

                if (line.signal >= CMD_SIGNALS.TAB || executions == 0 || routine == null)
                    if (command._commands.Count > 0)
                    {
                        if (command.TryReadCommand(line, out var path))
                        {
                            Executor exe = new(path, line, out bool err);
                            if (!err)
                                return routine = exe.Executate(line);
                        }
                        else if (line.signal == CMD_SIGNALS.EXEC)
                            Debug.LogWarning($"Could not find '{line.arg_last}' in '{cmd_name}' ({cmd_path})");
                        return null;
                    }

                if (line.signal == CMD_SIGNALS.EXEC)
                {
                    SaveHistory(line.text);
                    command.action?.Invoke(this);

                    if (executions == 0)
                    {
                        if (command.routine != null)
                        {
                            routine = command.routine(this);
                            routine.MoveNext();
                            return routine;
                        }
                    }
                    else if (routine != null)
                    {
                        temp_line = line;
                        if (!routine.MoveNext())
                            routine = null;
                        temp_line = null;
                    }
                }

                return null;
            }

            //--------------------------------------------------------------------------------------------------------------

            public void Dispose()
            {
                lock (disposed)
                {
                    if (disposed._value)
                        return;
                    disposed._value = true;

                    OnDispose();
                }
            }

            protected virtual void OnDispose()
            {
            }
        }
    }
}