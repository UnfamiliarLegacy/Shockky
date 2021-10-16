namespace Shockky.Lingo.Instructions
{
    public class GetMovieInfoIns : VariableReference
    {
        private int _valueIndex;
        public int ValueIndex
        {
            get => _valueIndex;
            set
            {
                _valueIndex = value;
            }
        }

        public GetMovieInfoIns(LingoFunction function)
            : base(OPCode.GetMovieInfo, function)
        {
            IsMovieReference = true;
        }
        public GetMovieInfoIns(LingoFunction function, int nameIndex)
            : this(function)
        {
            ValueIndex = nameIndex;
        }
        public GetMovieInfoIns(LingoFunction function, string name)
            : this(function)
        {
            Name = name;
        }

        public override int GetPopCount() => 1;
    }
}