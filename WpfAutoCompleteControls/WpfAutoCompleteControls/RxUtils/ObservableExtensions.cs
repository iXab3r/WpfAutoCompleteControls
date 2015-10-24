namespace WpfAutoCompleteControls.RxUtils
{
    using System;

    internal static class ObservableExtensions
    {
        public static ISuspendableObservable<T> ToSuspendable<T>(this IObservable<T> observable)
        {
            return new SuspendableObservable<T>(observable);
        }

        public static IDisposable Subscribe<TThis,TOther>(this IObservable<TThis> observable, Action<TThis> onNext, ISuspendableObservable<TOther> observableToSuspend)
        {
            return observable.Subscribe(
                x =>
                {
                    using (observableToSuspend.Suspend())
                    {
                        onNext(x);
                    }
                });
        }
    }
}