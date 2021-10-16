namespace Shockky.Lingo.Instructions
{
    public class SetParameterIns : VariableAssignment
    {
        private int _argumentNameIndex;
        public int ArgumentNameIndex
        {
            get => _argumentNameIndex;
            set
            {
                _argumentNameIndex = value;
            }
        }

        public SetParameterIns(LingoFunction function)
            : base(OPCode.SetParameter, function)
        { }
        public SetParameterIns(LingoFunction function, int argumentNameIndex)
            : this(function)
        {
            ArgumentNameIndex = argumentNameIndex;
        }
    }
}