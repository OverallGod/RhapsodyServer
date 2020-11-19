using System;

namespace RhapsodyServer.Event.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class DialogAttribute : Attribute
    {
        public string DialogName { get; }

        public DialogAttribute(string dialogName)
        {
            DialogName = dialogName;
        }

    }
}
