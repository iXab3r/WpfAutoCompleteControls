namespace WpfAutoCompleteControls.RxUtils
{
    using System;

    internal static class ObservableExtensions
    {
        public static ISuspendableObservable<T> ToSuspendable<T>(this IObservable<T> observable)
        {
            return new SuspendableObservable<T>(observable);
        }
    }
}