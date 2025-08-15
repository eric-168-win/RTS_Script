using System;
using System.Collections.Generic;
using RTS_LEARN.Units;

namespace RTS_LEARN.EventBus
{
    public static class Bus<T> where T : IEvent
    {
        public delegate void Event(T args);
        //delegate is a type that defines a method signature, 
        //which can be used to create methods that match this signature
        //Event is a delegate type that takes a parameter of type T and returns void

        // public static event Event OnEvent;
        // //(event means that it can only be invoked from within the class)
        // public static void Raise(T evt) => OnEvent?.Invoke(evt);
        // public static void Raise(T evt)
        // {
        //     OnEvent?.Invoke(evt);
        // }

        public static void Raise(Owner owner, T evt) => OnEvent[owner]?.Invoke(evt);

        public static Dictionary<Owner, Event> OnEvent = new()
        {
            { Owner.Player1, null },
            { Owner.AI1, null },
            { Owner.AI2, null },
            { Owner.AI3, null },
            { Owner.AI4, null },
            { Owner.AI5, null },
            { Owner.AI6, null },
            { Owner.AI7, null },
            { Owner.Invalid, null },
            { Owner.Unowned, null }
        };
        public static void RegisterForAll(Event handler)
        {
            foreach (Owner owner in Enum.GetValues(typeof(Owner)))
            {
                OnEvent[owner] += handler;
            }
        }

        public static void UnregisterForAll(Event handler)
        {
            foreach (Owner owner in Enum.GetValues(typeof(Owner)))
            {
                OnEvent[owner] -= handler;
            }
        }

    }
}