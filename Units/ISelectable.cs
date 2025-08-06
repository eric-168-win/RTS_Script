namespace RTS_LEARN.Units
{
    public interface ISelectable
    {
        bool /*public by default*/ IsSelected { get; }
        /*public by default*/
        void Select();
        /*public by default*/ void Deselect();
    }
}