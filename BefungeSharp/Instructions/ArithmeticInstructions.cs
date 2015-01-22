﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Arithmetic
{
    /*Arithmetic
        case '+':
        case '-'://Subtract b-a       
        case '*':
        case '/'://Divide b/a                        
        case '%'://modulous b % a
    */
    public abstract class ArithmeticInstructions : Instruction
    {
        public ArithmeticInstructions(char inName, UInt32 minimum_flags) : base(inName, CommandType.Arithmetic, ConsoleColor.Green, minimum_flags) { }

    }

    /// <summary>
    /// The add instruction, pops a and b then puts a + b onto the stack
    /// </summary>
    public class AddInstruction : ArithmeticInstructions
    {
        public AddInstruction(char inName, UInt32 minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip, BoardManager mgr = null)
        {
            base.EnsureStackSafety(ip.Stack, 2);
            ip.Stack.Push(ip.Stack.Pop() + ip.Stack.Pop());
            return true;
        }
    }

    /// <summary>
    /// The subtract instruction, pops a and b then puts b - a onto the stack
    /// </summary>
    public class SubtractInstruction : ArithmeticInstructions
    {
        public SubtractInstruction(char inName, UInt32 minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip, BoardManager mgr = null)
        {
            base.EnsureStackSafety(ip.Stack, 1);
            int a = ip.Stack.Pop();
            int b = ip.Stack.Pop();
            ip.Stack.Push(b - a);
            return true;
        }
    }

    /// <summary>
    /// The Multiplyinstruction, pops a and b then puts a * b onto the stack
    /// </summary>
    public class MultiplyInstruction : ArithmeticInstructions
    {
        public MultiplyInstruction(char inName, UInt32 minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip, BoardManager mgr = null)
        {
            base.EnsureStackSafety(ip.Stack, 2);
            ip.Stack.Push(ip.Stack.Pop() * ip.Stack.Pop());
            return true;
        }
    }

    /// <summary>
    /// The divide instruction, pops a and b then puts b/a, integer division only, onto the stack
    /// </summary>
    public class DivideInstruction : ArithmeticInstructions
    {
        public DivideInstruction(char inName, UInt32 minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip, BoardManager mgr = null)
        {
            base.EnsureStackSafety(ip.Stack, 2);
            int a = ip.Stack.Pop();
            int b = ip.Stack.Pop();
            //TODO - does this follow this "the / "Divide" instruction, which pops two values, divides the second by the first using integer division, and pushes the result (note that division by zero produces a result of zero in Funge-98, but Befunge-93 instead is supposed to ask the user what they want the result of the division to be); and"
            if (a != 0)
            {
                int result = b / a;
                ip.Stack.Push(result);
            }
            else
            {
                ip.Stack.Push(0);
            }
            return true;
        }
    }

    /// <summary>
    /// The modulo instruction, pops a and b then puts b % a onto the stack
    /// </summary>
    public class ModuloInstruction : ArithmeticInstructions
    {
        public ModuloInstruction(char inName, UInt32 minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip, BoardManager mgr = null)
        {
            base.EnsureStackSafety(ip.Stack, 2);

            int a = ip.Stack.Pop();
            int b = ip.Stack.Pop();

            //TODO does this follow the spec?the % "Remainder" instruction, which pops two values, divides the second by the first using integer division, and pushes the remainder, of those. Remainder by zero is subject to the same rules as division by zero, but if either argument is negative, the result is implementation-defined.
            if (a != 0)
            {
                int result = b % a;
                ip.Stack.Push(result);
            }
            else
            {
                ip.Stack.Push(0);
            }
            return true;
        }
    }
                   
    
}
