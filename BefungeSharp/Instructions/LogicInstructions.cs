﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Logic
{
    //Logic
    /*
        case '!'://not
        case '_':
        case '|':
        case '`'://Greater than 
        case 'w'://Funge98 compare function
            break;
    */
    public abstract class LogicInstruction : Instruction, IRequiresPop
    {
        protected int requiredCells;
        public LogicInstruction(char inName, int minimum_flags) : base(inName, CommandType.Logic, ConsoleColor.DarkGreen, minimum_flags) { }

        public int RequiredCells()
        {
            return requiredCells;
        }
    }

    public class NotInstruction : LogicInstruction
    {
        public NotInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { this.requiredCells = 1; }

        public override bool Preform(IP ip)
        {
            base.EnsureStackSafety(ip.Stack, this.RequiredCells());
            if (ip.Stack.Pop() != 0)
            {
                ip.Stack.Push(0);
            }
            else
            {
                ip.Stack.Push(1);
            }
            return true;
        }
    }

    public class HorizontalIfInstruction : LogicInstruction
    {
        public HorizontalIfInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { this.requiredCells = 1; }

        public override bool Preform(IP ip)
        {
            base.EnsureStackSafety(ip.Stack, this.RequiredCells());
            if (ip.Stack.Pop() == 0)
            {
                ip.Delta = Vector2.East;
            }
            else
            {
                ip.Delta = Vector2.West;
            }
            return true;
        }
    }

    public class VerticalIfInstruction : LogicInstruction
    {
        public VerticalIfInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { this.requiredCells = 1; }

        public override bool Preform(IP ip)
        {
            base.EnsureStackSafety(ip.Stack, this.RequiredCells());
            if (ip.Stack.Pop() == 0)
            {
                ip.Delta = Vector2.South;
            }
            else
            {
                ip.Delta = Vector2.North;
            }
            return true;
        }
    }

    public class GreaterThanInstruction : LogicInstruction
    {
        public GreaterThanInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { this.requiredCells = 1; }

        public override bool Preform(IP ip)
        {
            base.EnsureStackSafety(ip.Stack, this.RequiredCells());
            
            int a = ip.Stack.Pop();
            int b = ip.Stack.Pop();

            if (b > a)
            {
                ip.Stack.Push(1);
            }
            else
            {
                ip.Stack.Push(0);
            }
            
            return true;
        }
    }

    public class CompareInstruction : LogicInstruction
    {
        public CompareInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { this.requiredCells = 1; }

        public override bool Preform(IP ip)
        {
            base.EnsureStackSafety(ip.Stack, this.RequiredCells());
            //Pop a and b off the stack
            int a = ip.Stack.Pop();
            int b = ip.Stack.Pop();

            //Get our current direction
            Vector2 currentDir = ip.Delta;

            if (b < a)//If b is less than turn left
            {
                ip.Delta = new Vector2(ip.Delta.y * -1, ip.Delta.x);
            }
            else if (b > a)//if b is more turn right
            {
                ip.Delta = new Vector2(ip.Delta.y, ip.Delta.x * -1);
            }
            return true;
        }
    }
}
