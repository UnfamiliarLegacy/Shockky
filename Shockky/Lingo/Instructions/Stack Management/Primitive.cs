using Shockky.IO;

namespace Shockky.Lingo.Instructions
{
    public abstract class Primitive : Instruction
    {
        private object _value;
        new public object Value
        {
            get => _value;
            set
            {
                _value = value;
            }
        }

        protected Primitive(OPCode op)
            : base(op)
        { }
        protected Primitive(OPCode op, LingoFunction function)
            : base(op, function)
        { }

        public override int GetPushCount() => 1;

        public override void WriteTo(ShockwaveWriter output)
        {
            base.WriteTo(output);
        }

        public static bool IsValid(OPCode op)
        {
            switch (op)
            {
                case OPCode.PushInt0:
                case OPCode.PushInt:
                case OPCode.PushInt16:
                case OPCode.PushInt32:
                case OPCode.PushFloat:
                case OPCode.PushConstant:
                case OPCode.PushSymbol:
                    return true;
                default:
                    return false;
            }
        }
        public static Primitive Create(LingoFunction function, object value)
        {
            return Type.GetTypeCode(value.GetType()) switch
            {
                TypeCode.Byte => new PushIntIns(function, (byte)value),
                TypeCode.Int16 => new PushIntIns(function, (short)value),
                TypeCode.Int32 => new PushConstantIns(function, (int)value),
                TypeCode.Int64 => new PushFloatIns(function, (float)value),

                TypeCode.String => new PushConstantIns(function, (string)value),
                //case TypeCode.Double => return new PushConstantIns(abc, (double)value),
               
                // case TypeCode.Empty => new PushNullIns(),
                _ => null
            };
        }
    }
}