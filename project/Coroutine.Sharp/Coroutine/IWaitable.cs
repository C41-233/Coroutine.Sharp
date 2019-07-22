﻿using System;
using Coroutine.Wait;

namespace Coroutine
{
    public interface IWaitable
    {

        WaitableStatus Status { get; }

        Exception Exception { get; }

        IWaitable OnSuccess(Action callback);

        IWaitable OnFail(Action<Exception> callback);

        void Abort();
    }

    public interface IWaitable<out T> : IWaitable
    {

        T Result { get; }

        IWaitable<T> OnSuccess(Action<T> callback);

    }

    public interface IWaitable<out T1, out T2> : IWaitable
    {
        T1 Result1 { get; }

        T2 Result2 { get; }

        IWaitable<T1, T2> OnSuccess(Action<T1, T2> callback);
    }

    public static class WaitableExtends
    {

        public static IWaitable Co(this IWaitable self, out IWaitable waitable)
        {
            waitable = self;
            return self;
        }

        public static IWaitable EnsureSuccess(this IWaitable self)
        {
            if (self.Exception == null)
            {
                return self;
            }
            throw new Exception(null, self.Exception);
        }

        public static IWaitable OnFail(this IWaitable self, Action callback)
        {
            if (callback == null)
            {
                return self;
            }
            return self.OnFail(e => callback());
        }

        public static IWaitable PreventCaptureAbort(this IWaitable self)
        {
            return new PreventAbortWaitable(self);
        }

    }

}