namespace Shockky.Lingo.Instructions
{
    public class GetPropertyIns : VariableReference
    {
        private int _propertyNameIndex;
        public int PropertyNameIndex
        {
            get => _propertyNameIndex;
            set
            {
                _propertyNameIndex = value;
            }
        }

        public GetPropertyIns(LingoFunction function)
            : base(OPCode.GetProperty, function)
        { }
        public GetPropertyIns(LingoFunction function, int propertyNameIndex)
            : this(function)
        {
            PropertyNameIndex = propertyNameIndex;
        }
    }
}