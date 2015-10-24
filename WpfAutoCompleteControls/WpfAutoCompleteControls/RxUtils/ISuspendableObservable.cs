using System;

namespace WpfAutoCompleteControls.RxUtils
{
    internal interface ISuspendableObservable<out T> : IObservable<T>
    {
        IDisposable Suspend();
    }
}