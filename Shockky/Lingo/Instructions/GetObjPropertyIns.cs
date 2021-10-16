namespace Shockky.Lingo.Instructions
{
    public class GetObjPropertyIns : VariableReference
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

        public GetObjPropertyIns(LingoFunction function)
            : base(OPCode.GetObjProp, function)
        {
            IsObjectReference = true;
        }
        public GetObjPropertyIns(LingoFunction function, int propertyNameIndex)
            : this(function)
        {
            PropertyNameIndex = propertyNameIndex;
        }
        public GetObjPropertyIns(LingoFunction function, string propertyName)
            : this(function)
        {
            Name = propertyName;
        }

        public override int GetPopCount() => 1;
    }
}