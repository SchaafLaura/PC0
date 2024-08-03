namespace PC0
{
    internal class VariableList<T> : List<T>
    {
        int hash;

        public VariableList() : base() { }

        public VariableList(VariableList<T> list) : base(list) 
        {
            SetHashCode();
        }

        public void SetHashCode()
        {
            hash = string.Join(string.Empty, this).GetHashCode();
        }

        public override int GetHashCode()
        {
            return hash;
        }

        public override bool Equals(object? obj)
        {
            return 
                obj is VariableList<T> other &&
                other.hash == hash;
        }

        public override string ToString()
        {
            return string.Join(string.Empty, this);
        }
    }
}
