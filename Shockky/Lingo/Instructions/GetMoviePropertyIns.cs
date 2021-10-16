namespace Shockky.Lingo.Instructions
{
    public class GetMoviePropertyIns : VariableReference
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

        public GetMoviePropertyIns(LingoFunction function)
            : base(OPCode.GetMovieProp, function)
        {
            IsMoviePropertyReference = true;
        }
        public GetMoviePropertyIns(LingoFunction function, int propertyNameIndex)
            : this(function)
        {
            ValueIndex = propertyNameIndex;
        }
        public GetMoviePropertyIns(LingoFunction function, string moviePropertyName)
            : this(function)
        {
            Name = moviePropertyName;
        }
    }
}