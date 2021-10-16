namespace Shockky.Lingo.Instructions
{
    public class SetObjPropertyIns : VariableAssignment
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

        public SetObjPropertyIns(LingoFunction function)
            : base(OPCode.SetObjProp, function)
        { }
        public SetObjPropertyIns(LingoFunction function, int propertyNameIndex)
            : this(function)
        {
            PropertyNameIndex = propertyNameIndex;
        }

        public override int GetPopCount() => 2;
    }
}