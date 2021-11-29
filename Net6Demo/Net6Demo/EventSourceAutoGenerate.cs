namespace System.Diagnostics.Tracing
{
    [EventSource(Guid = "49592C0F-5A05-516D-AA4B-A64E02026C89", Name = "System.Runtime")]
    [EventSourceAutoGenerateAttribute]
    //[System.Diagnostics.Tracing.EventSourceAutoGenerateAttribute]
    internal sealed partial class RuntimeEventSource : EventSource
    {
    }

    [AttributeUsage(AttributeTargets.All)]
    sealed class EventSourceAutoGenerateAttribute : Attribute { }
}
