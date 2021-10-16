namespace Shockky.Lingo.Instructions
{
    public class SetPropertyIns : VariableAssignment
    {
        private int _propertNameIndex;
        public int PropertyNameIndex
        {
            get => _propertNameIndex;
            set
            {
                _propertNameIndex = value;
            }
        }

        public SetPropertyIns(LingoFunction function)
            : base(OPCode.SetProperty, function)
        { }
        public SetPropertyIns(LingoFunction function, int propertyNameIndex)
            : this(function)
        {
            PropertyNameIndex = propertyNameIndex;
        }
    }
}