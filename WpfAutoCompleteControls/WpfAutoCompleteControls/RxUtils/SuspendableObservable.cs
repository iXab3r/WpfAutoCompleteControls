namespace WpfAutoCompleteControls.RxUtils
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Threading;

    internal sealed class SuspendableObservable<T> : ISuspendableObservable<T>
    {
        private readonly IObservable<T> observable;
        private int isSuspended;

        public SuspendableObservable(IObservable<T> observable)
        {
            this.observable = observable.Where(_ => isSuspended == 0);
        }

        public IDisposable Suspend()
        {
            Interlocked.Increment(ref isSuspended);
            return Disposable.Create(() => Interlocked.Decrement(ref isSuspended));
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return observable.Subscribe(observer);
        }
    }
}