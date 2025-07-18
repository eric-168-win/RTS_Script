namespace RTS_LEARN.EventBus
{
    public static class Bus<T> where T : IEvent
    {
        public delegate void Event(T args);
        //delegate is a type that defines a method signature, 
        //which can be used to create methods that match this signature
        //Event is a delegate type that takes a parameter of type T and returns void

        public static event Event OnEvent; //(event means that it can only be invoked from within the class)

        public static void Raise(T evt) => OnEvent?.Invoke(evt);
        // public static void Raise(T evt)
        // {
        //     OnEvent?.Invoke(evt);
        // }

    }
}