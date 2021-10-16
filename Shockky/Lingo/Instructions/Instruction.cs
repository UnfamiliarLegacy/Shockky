using System.Diagnostics;

using Shockky.IO;

namespace Shockky.Lingo.Instructions
{
    [DebuggerDisplay("{OP}")]
    public abstract class Instruction : ShockwaveItem, ICloneable
    {
        public OPCode OP { get; }
        public virtual int Value { protected get; set; }

        protected LingoFunction Function { get; }

        protected Instruction(OPCode op)
        {
            OP = op;
        }
        protected Instruction(OPCode op, LingoFunction function)
            : this(op)
        {
            Function = function;
        }

        public override void WriteTo(ShockwaveWriter output)
        {
            byte op = (byte)OP;

            if (Value > byte.MaxValue)
                op += 0x40;

            if (Value > short.MaxValue)
                op += 0x40;

            output.Write(op);

            if (op > 0xC0) output.Write(Value);
            else if (op > 0x80) output.Write((short)Value);
            else if (op > 0x40) output.Write((byte)Value);
        }

        public virtual int GetPopCount() => 0;
        public virtual int GetPushCount() => 0;

        public override int GetBodySize() => throw new NotImplementedException();

        public static Instruction Create(ref ShockwaveReader input)
        {
            int operandValue = 0;

            LingoFunction function = null; //TODO:

            byte op = input.ReadByte();
            byte ogOp = op;

            if (op > 0x40)
            {
                op %= 0x40;
                op += 0x40;

                if (ogOp > 0xC0)
                    operandValue = input.ReadInt32();
                else if (ogOp > 0x80) operandValue = input.ReadInt16();
                else operandValue = input.ReadByte();
            }

            //Debug.WriteLine($"{ogOp:X2}|{Enum.GetName(typeof(OPCode), (OPCode)op)} {(op > 0x40 ? operandValue.ToString() : string.Empty)}");

            return (OPCode)op switch
            {
                OPCode.Return => new ReturnIns(),
                OPCode.PushInt0 => new PushZeroIns(),
                OPCode.Multiple => new MultipleIns(),
                OPCode.Add => new AddIns(),
                OPCode.Substract => new SubtractIns(),
                OPCode.Divide => new DivideIns(),
                OPCode.Modulo => new ModuloIns(),
                OPCode.Inverse => new InverseIns(),
                OPCode.JoinString => new JoinStringIns(),
                OPCode.JoinPadString => new JoinPadStringIns(),
                OPCode.LessThan => new LessThanIns(),
                OPCode.LessThanEquals => new LessEqualsIns(),
                OPCode.NotEqual => new NotEqualIns(),
                OPCode.Equals => new EqualsIns(),
                OPCode.GreaterThan => new GreaterThanIns(),
                OPCode.GreaterThanEquals => new GreaterEqualsIns(),
                OPCode.And => new AndIns(),
                OPCode.Or => new OrIns(),
                OPCode.Not => new NotIns(),
                OPCode.ContainsString => new ContainsStringIns(),
                OPCode.StartsWith => new StartsWithIns(),
                OPCode.ChunkExpression => new ChunkExpressionIns(),
                OPCode.Hilite => new HiliteIns(),
                OPCode.OntoSprite => new OntoSpriteIns(),
                OPCode.IntoSprite => new IntoSpriteIns(),
                OPCode.CastString => new CastStringIns(),
                OPCode.StartTell => new StartTellIns(),
                OPCode.EndTell => new EndTellIns(),
                OPCode.WrapList => new WrapListIns(),
                OPCode.NewPropList => new NewPropListIns(),
                OPCode.Swap => new SwapIns(),

                //Multi 
                OPCode.PushInt => new PushIntIns(function, operandValue),
                OPCode.NewArgList => new NewListIns(function, operandValue, true), //unparanthesized
                OPCode.NewList => new NewListIns(function, operandValue, false), //in paranthesized call expression
                OPCode.PushConstant => new PushConstantIns(function, operandValue),
                OPCode.PushSymbol => new PushSymbolIns(function, operandValue),
                OPCode.PushObject => new PushObjectIns(function, operandValue),
                OPCode.GetGlobal => new GetGlobalIns(function, operandValue),
                OPCode.GetProperty => new GetPropertyIns(function, operandValue),
                OPCode.GetParameter => new GetParameterIns(function, operandValue),
                OPCode.GetLocal => new GetLocalIns(function, operandValue),
                OPCode.SetGlobal => new SetGlobalIns(function, operandValue),
                OPCode.SetProperty => new SetPropertyIns(function, operandValue),
                OPCode.SetParameter => new SetParameterIns(function, operandValue),
                OPCode.SetLocal => new SetLocalIns(function, operandValue),
                OPCode.Jump => new JumpIns(function, operandValue),
                OPCode.EndRepeat => new EndRepeatIns(function, operandValue),
                OPCode.IfFalse => new IfFalseIns(function, operandValue),
                OPCode.CallLocal => new CallLocalIns(function, operandValue),
                OPCode.CallExternal => new CallExternalIns(function, operandValue),
                OPCode.InsertString => new InsertStringIns(function, operandValue),
                OPCode.Insert => new InsertIns(function, operandValue),
                OPCode.Get => new GetIns(function, operandValue),
                OPCode.Set => new SetIns(function, operandValue),
                OPCode.GetMovieProp => new GetMoviePropertyIns(function, operandValue),
                OPCode.SetMovieProp => new SetMoviePropertryIns(function, operandValue),
                OPCode.GetObjProp => new GetObjPropertyIns(function, operandValue),
                OPCode.SetObjProp => new SetObjPropertyIns(function, operandValue),
                OPCode.Dup => new DupIns(operandValue),
                OPCode.Pop => new PopIns(operandValue),
                OPCode.GetMovieInfo => new GetMovieInfoIns(function, operandValue),
                OPCode.CallObj => new CallObjectIns(function, operandValue),
                OPCode.PushInt16 => new PushIntIns(function, operandValue),
                OPCode.PushInt32 => new PushIntIns(function, operandValue),
                OPCode.PushFloat => new PushFloatIns(function, operandValue),
                OPCode.ReturnFactory => throw new NotImplementedException(),
                
                OPCode.Op_47 => throw new NotImplementedException(),
                OPCode.Op_48 => throw new NotImplementedException(),
                OPCode.Op_4d => throw new NotImplementedException(),
                OPCode.Op_4e => throw new NotImplementedException(),
                OPCode.CallObjOld => throw new NotImplementedException(),
                OPCode.DeleteString => throw new NotImplementedException(),
                OPCode.Op_5E => throw new NotImplementedException(),
                OPCode.TellCall => throw new NotImplementedException(),
                OPCode.Op_68 => throw new NotImplementedException(),
                OPCode.Op_69 => throw new NotImplementedException(),
                OPCode.Op_6A => throw new NotImplementedException(),
                OPCode.Op_6B => throw new NotImplementedException(),
                OPCode.Op_6C => throw new NotImplementedException(),
                OPCode.Op_6D => throw new NotImplementedException(),
                OPCode.GetChainedProp => throw new NotImplementedException(),
                OPCode.GetTopLevelProp => throw new NotImplementedException(),
                OPCode.Op_73 => throw new NotImplementedException(),
                OPCode.Op_7d => throw new NotImplementedException(),
                //OPCode.Op_72:
                //Operand points to names prefixed by "_", is this another special movie property get? 
                //TODO: inspect stack at this OP. 
                //string _prefixed = handler.Script.Pool.GetName(operandValue), //Occurred values: _movie, _global, _system
            };
        }

        object ICloneable.Clone() => Clone();
        public Instruction Clone() => (Instruction)MemberwiseClone();
    }
}