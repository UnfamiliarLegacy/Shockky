namespace Shockky.Lingo.Instructions
{
    public class SetMoviePropertryIns : VariableAssignment
    {
        private int _nameIndex;
        public int NameIndex
        {
            get => _nameIndex;
            set
            {
                _nameIndex = Value = value;
            }
        }

        public SetMoviePropertryIns(LingoFunction function)
            : base(OPCode.SetMovieProp, function)
        {
            IsMoviePropertyReference = true;
        }
        public SetMoviePropertryIns(LingoFunction function, int moviePropertyIndex)
            : this(function)
        {
            NameIndex = moviePropertyIndex;
        }
    }
}