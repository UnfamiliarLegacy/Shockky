﻿using Shockky.Shockwave.Lingo.Bytecode.Instructions.Enum;

namespace Shockky.Shockwave.Lingo.Bytecode.Instructions
{
    public class SubtractIns : Computation
    {
        public SubtractIns() 
            : base(OPCode.Substract)
        { }

     /*   protected override object Execute(dynamic left, dynamic right)
        {
            return left - right;
        }*/
    }
}
