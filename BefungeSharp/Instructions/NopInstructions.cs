﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Nop
{
    public abstract class NopInstruction : Instruction
    {
        public NopInstruction(char inName, int minimum_flags) : base(inName, CommandType.Nop, ConsoleColor.Black, minimum_flags) { }
    }

    public class SpaceInstruction : NopInstruction
    {
        public SpaceInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            /* Move to the next space,
             * if we have found a ';', preform the jumping over of ethereal space
             * Stop if we have found a non-space character
             */
            do
            {
                ip.Move();
                int current = ip.GetCurrentCell().value;
                if (current == ';')
                {
                    InstructionManager.InstructionSet[';'].Preform(ip);
                }
            }
            while (ip.GetCurrentCell().value == ' ');
            
            return true;
        }
    }

    public class ExplicitNopInstruction : NopInstruction
    {
        public ExplicitNopInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { this.color = ConsoleColor.DarkBlue; }

        public override bool Preform(IP ip)
        {
            //The simplest instruction of all, do nothing!
            return true;
        }
    }
}
