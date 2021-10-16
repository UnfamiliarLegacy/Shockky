namespace Shockky.Lingo.Instructions
{
    public class GetParameterIns : VariableReference
    {
        private int _nameIndex;
        public int NameIndex
        {
            get => _nameIndex;
            set
            {
                _nameIndex = value;
            }
        }

        public GetParameterIns(LingoFunction function)
            : base(OPCode.GetParameter, function)
        { }
        public GetParameterIns(LingoFunction function, int argumentNameIndex)
            : this(function)
        {
            NameIndex = argumentNameIndex;
        }
    }
}