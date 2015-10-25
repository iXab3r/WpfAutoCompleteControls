namespace WpfAutoCompleteControls.RxUtils
{
    using System;
    using System.Reactive.Linq;

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

        public static IDisposable Subscribe<TThis>(this IObservable<TThis> observable, Action onNext)
        {
            return observable.Subscribe(_ => onNext());
        }

        public static IObservable<TThis> Do<TThis>(this IObservable<TThis> observable, Action onNext)
        {
            return observable.Do(_ => onNext());
        }
    }
}