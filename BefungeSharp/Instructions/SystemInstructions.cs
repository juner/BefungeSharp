﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BefungeSharp.Instructions.SystemCalls
{
    public abstract class SystemInstruction : Instruction
    {
        public SystemInstruction(char inName, int minimum_flags) : base(inName, CommandType.IO, ConsoleColor.DarkMagenta, minimum_flags) { }
    }

    public class ExecuteInstruction : SystemInstruction, IRequiresPop
    {
        public ExecuteInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            base.EnsureStackSafety(ip.Stack, RequiredCells());
            
            //Pop a command that will be fed into cmd, /k forces the window to stay open
            string command = StackUtils.StringPop(ip.Stack,true);

            //Assume we've failed
            int exitCode = -1;
            
            Process cmd = new Process();
            
            //Try to open a process
            try
            {
                cmd = Process.Start("cmd.exe", "/k" + command);
                //Pause the execution of this program to wait
                cmd.WaitForExit();
                exitCode = cmd.ExitCode;
            }
            catch(Exception e)
            {
                //If it fails
                if (cmd.ExitCode != 0)
                {
                    ip.Negate();
                    ip.Stack.Push(cmd.ExitCode);
                    return false;
                }
            }
            
            ip.Stack.Push(0);
            return true;
        }

        public int RequiredCells()
        {
            //Requires to pop atleast a 0, aka null string
            return 1;
        }
    }

    public class GetSysInfo : SystemInstruction, IRequiresPop
    {
        public GetSysInfo(char inName, int minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            //Instead of putting all 20 values on the stack only to remove 19 or 20 of them we simply
            //imitate the end result and move on. If someone can tell me why the spec should be followed to the
            //letter please e-mail the author. They would greatly appreciate it.
            
            base.EnsureStackSafety(ip.Stack, RequiredCells());
            int initialTOSS_Size = ip.Stack.Count;

            //Which option we will start examining
            int toExamine = ip.Stack.Pop();
            //Start with assuming we're just interested in one of them
            int toTake = 1;

            //If it is less than or equal to zero we are taking the whole deal
            if (toExamine <= 0)
            {
                toTake = 20;
            }
            
            while (toTake > 0)
            {
                switch (toExamine)
                {
                    case 20:
                        //20. A series of strings containing the environment variables (global environment)
                        {
                            foreach (System.Collections.DictionaryEntry entry in System.Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine))
                            {
                                StackUtils.StringPush(ip.Stack, entry.Key.ToString().ToUpper() + "=" + entry.Value.ToString().ToUpper());
                            }

                            foreach (System.Collections.DictionaryEntry entry in System.Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process))
                            {
                                StackUtils.StringPush(ip.Stack, entry.Key.ToString().ToUpper() + "=" + entry.Value.ToString().ToUpper());
                            }

                            foreach (System.Collections.DictionaryEntry entry in System.Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User))
                            {
                                StackUtils.StringPush(ip.Stack, entry.Key.ToString().ToUpper() + "=" + entry.Value.ToString().ToUpper());
                            }
                        }
                        break;
                    case 19:
                        //19. A sequence of strings containing the file name, followed by all of the commandline arguments
                        //Passed into the interpreter.Each string is, of course, terminated with a '\0' and the whole is terminated with
                        //'\0''\0'. Two null string arguments will result in the command end parsing too early. This is rare
                        // (global environment)
                        {
                            StackUtils.StringPush(ip.Stack, "Your File Name Here!");

                            //Get an array of arguments from the current process's Arguments split on every space
                            string[] arguments = Process.GetCurrentProcess().StartInfo.Arguments.Split(new char[] { ' ' });

                            for (int i = 0; i < arguments.Length; i++)
                            {
                                StackUtils.StringPush(ip.Stack, arguments[i]);
                            }
                            ip.Stack.Push(0);
                            ip.Stack.Push(0);
                        }
                        break;
                    case 18:
                        //18. Size of each stack in the stack stack listed from TOSS to BOSS (ip specific)
                        {
                            //iterate through the whole stack stack, top to bottom
                            //TODO when stack stack is implemented - for each stack get it's count and push it, if it is the TOSS push intialTOSS_Size instead
                            ip.Stack.Push(initialTOSS_Size);
                        }
                        break;
                        //17. Number of stacks on the stack stack (ip specific)
                        {
                            //TODO when stack stack is implemented
                            //For now, push the count of the one stack we have
                            ip.Stack.Push(1);
                        }
                    case 16:
                        //16. The current hour, minute, and second (local environment)
                        {
                            System.DateTime time = System.DateTime.Now;
                            int hours = time.Hour * 256 * 256;
                            int minutes = time.Minute * 256;
                            int seconds = time.Second;

                            ip.Stack.Push(hours + minutes + seconds);
                        }
                        break;
                    case 15:
                        //15. The current year, month, and day (local environment)
                        {
                            System.DateTime time = System.DateTime.Now;
                            int year = (time.Year - 1900) * 256 * 256;
                            int month = time.Month * 256;
                            int day = time.Day;

                            ip.Stack.Push(year + month + day);
                        }
                        break;
                    case 14:
                        //14. A vector pointing to the greatest non-empty space relative to the least point
                        //If you were to have a non-empty cell at 79, 24 this point is 0 + 79, 0 + 24 (local environment)
                        {
                            //For now we push 79, 24 until we create the advanced funge space system
                            StackUtils.VectorPush(ip.Stack, new Vector2(0 + 79, 0 + 24));
                        }
                        break;
                    case 13:
                        //13. A vector pointing to the least non-empty space relative to the origin
                        {
                            StackUtils.VectorPush(ip.Stack, new Vector2(0 - 0, 0 - 0));
                        }
                        break;
                    case 12:
                        //12. A vector containing the storage offset of the ip (ip specific)
                        {
                            StackUtils.VectorPush(ip.Stack, ip.StorageOffset);
                        }
                        break;
                    case 11:
                        //11. A vector containing the delta of the ip (ip specific)
                        {
                            StackUtils.VectorPush(ip.Stack, ip.Delta);
                        }
                        break;
                    case 10:
                        //10. A vector containing the position of the ip (ip specific)
                        {
                            StackUtils.VectorPush(ip.Stack, ip.Position);
                        }
                        break;
                    case 9:
                        //9. A cell containing a unique team number (ip specific)
                        //This appears to be useless, not even appearing in RC/Funge
                        {
                            ip.Stack.Push(0);
                        }
                        break;
                    case 8:
                        //8. A cell containing the unique ID for the current ip (ip specific)
                        //Used in Concurrent Funge
                        {
                            ip.Stack.Push(ip.ID);
                        }
                        break;
                    case 7:
                        //7. A cell containing the dimensions of the interpreter
                        //1 for Unefunge, 2 for Befunge, 3 for Trefunge, etc. (global environment)
                        {
                            //For now we'll just push 2
                            ip.Stack.Push(2);
                        }
                        break;
                    case 6:
                        //6. A cell containing the path sperator for use with 'i' and 'o' (global environment)
                        {
                            //Give this is a windows system the seperator is that aweful \
                            ip.Stack.Push('\\');
                        }
                        break;
                    case 5:
                        //5. A cell containing an ID code for the Operating Paradigm,
                        //used for understanding how the '=' instruction will handle input (global environment)
                        {
                            //1 means C-system style calling
                            ip.Stack.Push(1);
                        }
                        break;
                    case 4:
                        //4. A cell containing this implementation's version number 
                        //where all .'s are stripped out (local environment)
                        {
                            //I'd say we are currently about half way through implementing the language and the UI
                            //Making the current version 4.7.0
                            ip.Stack.Push(47);
                        }
                        break;
                    case 3:
                        //3. A cell containing this implementation's handprint (local environment)
                        //Our handprint is BSHP for BefungeSharp! Oh so clever.
                        {
                            //0x42534850, aka 
                            //'B' = 66 = 0100 0010
                            //'S' = 83 = 0101 0011
                            //'H' = 72 = 0100 1000
                            //'P' = 80 = 0101 0000

                            //0100 0010 0101 0011 0100 1000 0101 0000
                            ip.Stack.Push(0x42534850);
                        }
                        break;
                    case 2:
                        //2. A cell containing the number of bytes per cell
                        {
                            ip.Stack.Push(sizeof(int));
                        }
                        break;
                    case 1:
                        //1. A cell containing various flags relating to which instructions
                        //Are implemented
                        {
                            ip.Stack.Push((int)flags);
                        }
                        break;
                    default:
                        {
                            //Since it is impossible to get stack[20-20]
                            //We must ensure we can atleast access stack[1]
                            base.EnsureStackSafety(ip.Stack, (toExamine - 20) + 1);
                            //If it is greater than 20 we will be "picking" off the stack
                            int result = ip.Stack.ElementAtOrDefault(toExamine - 20);
                            ip.Stack.Push(result);
                        }
                        break;
                }
                
                //If we are taking everything ever
                if (toTake == 20)
                {
                    //Decrease toExamine
                    toExamine--;
                }
                //Decrease the number to take
                toTake--;
            }
            return true;
        }

        public int RequiredCells()
        {
            return 1;
        }
    }
}
