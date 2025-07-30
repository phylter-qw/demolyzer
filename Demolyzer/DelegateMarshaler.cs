using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Demolyzer
{
    /// <summary>
    /// Provides the ability to marshal delegates and data from any thread to the thread in which the
    /// DelegateMarshaler was created using a SynchronizationContext object.
    /// This eliminates the need for having a reference to a control instance from
    /// the UI thread to use the Invoke/BeginInvoke methods.
    /// </summary>
    public sealed class DelegateMarshaler
    {
        /// <summary>
        /// The target thread's SynchronizationContext used for marshaling delegates and data
        /// </summary>
        private SynchronizationContext _synchronizationContext;

        /// <summary>
        /// Captures the current thread's SynchronizationContext (typically the UI's) and returns a new instance of the DelegateMarshaler
        /// </summary>
        /// <returns>A new instance of the DelegateMarshaler</returns>
        public static DelegateMarshaler Create()
        {
            // Throw an error if the synchronization context isn't defined.  This could happen if this is called
            // from a console application, before the UI thread is initialized, or from a worker thread.
            if (SynchronizationContext.Current == null)
            {
                throw new InvalidOperationException("No SynchronizationContext exists for the current thread.");
            }
            return new DelegateMarshaler(SynchronizationContext.Current);
        }

        /// <summary>
        /// Creates an instance of the DelegateMarshaler.  The Create method must be used to create the instance.
        /// </summary>
        /// <param name="synchronizationContext">The current thread's synchronization context.</param>
        private DelegateMarshaler(SynchronizationContext synchronizationContext)
        {
            this._synchronizationContext = synchronizationContext;
        }

        /// <summary>
        /// Gets whether the calling thread is already on the captured SynchronizationContext's thread.
        /// </summary>
        private bool IsMarshalRequired
        {
            get
            {
                return this._synchronizationContext != SynchronizationContext.Current;
            }
        }

        /// <summary>
        /// Calls a delegate with no parameters synchronously.  The call will be marshaled to the target thread
        /// if necessary.
        /// </summary>
        /// <param name="action">The parameterless delegate to invoke.</param>
        public void Invoke(Action action)
        {
            if (this.IsMarshalRequired == false)
            {
                // already on the target thread, just invoke delegate directly
                action();
            }
            else
            {
                // marshal the delegate call to the target thread
                this._synchronizationContext.Send(delegate { action(); }, null);
            }
        }

        /// <summary>
        /// Calls a delegate with one parameter synchronously.  The call will be marshaled to the target thread
        /// if necessary.
        /// </summary>
        /// <typeparam name="T">The type of object the delegate expects as a parameter.</typeparam>
        /// <param name="action">The delegate to invoke.</param>
        /// <param name="arg">The object to send as a parameter to the delegate.</param>
        public void Invoke<T>(Action<T> action, T arg)
        {
            if (this.IsMarshalRequired == false)
            {
                // already on the target thread, just invoke delegate directly
                action(arg);
            }
            else
            {
                // marshal the delegate call to the target thread
                this._synchronizationContext.Send(delegate { action(arg); }, null);
            }
        }

        /// <summary>
        /// Calls a delegate with two parameters synchronously.  The call will be marshaled to the target thread
        /// if necessary.
        /// </summary>
        /// <typeparam name="T">The type of object the delegate expects as a parameter.</typeparam>
        /// <param name="action">The delegate to invoke.</param>
        /// <param name="arg1">The object to send as the first parameter to the delegate.</param>
        /// <param name="arg2">The object to send as the second parameter to the delegate.</param>
        public void Invoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (this.IsMarshalRequired == false)
            {
                // already on the target thread, just invoke delegate directly
                action(arg1, arg2);
            }
            else
            {
                // marshal the delegate call to the target thread
                this._synchronizationContext.Send(delegate { action(arg1, arg2); }, null);
            }
        }

        /// <summary>
        /// Calls a delegate with three parameters synchronously.  The call will be marshaled to the target thread
        /// if necessary.
        /// </summary>
        /// <typeparam name="T">The type of object the delegate expects as a parameter.</typeparam>
        /// <param name="action">The delegate to invoke.</param>
        /// <param name="arg1">The object to send as the first parameter to the delegate.</param>
        /// <param name="arg2">The object to send as the second parameter to the delegate.</param>
        /// <param name="arg3">The object to send as the third parameter to the delegate.</param>
        public void Invoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if (this.IsMarshalRequired == false)
            {
                // already on the target thread, just invoke delegate directly
                action(arg1, arg2, arg3);
            }
            else
            {
                // marshal the delegate call to the target thread
                this._synchronizationContext.Send(delegate { action(arg1, arg2, arg3); }, null);
            }
        }

        /// <summary>
        /// Calls a delegate with four parameters synchronously.  The call will be marshaled to the target thread
        /// if necessary.
        /// </summary>
        /// <typeparam name="T">The type of object the delegate expects as a parameter.</typeparam>
        /// <param name="action">The delegate to invoke.</param>
        /// <param name="arg1">The object to send as the first parameter to the delegate.</param>
        /// <param name="arg2">The object to send as the second parameter to the delegate.</param>
        /// <param name="arg3">The object to send as the third parameter to the delegate.</param>
        /// <param name="arg4">The object to send as the fourth parameter to the delegate.</param>
        public void Invoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (this.IsMarshalRequired == false)
            {
                // already on the target thread, just invoke delegate directly
                action(arg1, arg2, arg3, arg4);
            }
            else
            {
                // marshal the delegate call to the target thread
                this._synchronizationContext.Send(delegate { action(arg1, arg2, arg3, arg4); }, null);
            }
        }

        /// <summary>
        /// Calls a delegate with no parameters asynchronously.  The call will be marshaled to the target thread
        /// if necessary.
        /// </summary>
        /// <param name="action">The parameterless delegate to invoke.</param>
        public void BeginInvoke(Action action)
        {
            if (this.IsMarshalRequired == false)
            {
                // already on the target thread, just invoke delegate directly
                action();
            }
            else
            {
                // marshal the delegate call to the target thread
                this._synchronizationContext.Post(delegate { action(); }, null);
            }
        }

        /// <summary>
        /// Calls a delegate with one parameter asynchronously.  The call will be marshaled to the target thread
        /// if necessary.
        /// </summary>
        /// <typeparam name="T">The type of object the delegate expects as a parameter.</typeparam>
        /// <param name="action">The delegate to invoke.</param>
        /// <param name="arg">The object to send as a parameter to the delegate.</param>
        public void BeginInvoke<T>(Action<T> action, T arg)
        {
            if (this.IsMarshalRequired == false)
            {
                // already on the target thread, just invoke delegate directly
                action(arg);
            }
            else
            {
                // marshal the delegate call to the target thread
                this._synchronizationContext.Post(delegate { action(arg); }, null);
            }
        }

        /// <summary>
        /// Calls a delegate with two parameters asynchronously.  The call will be marshaled to the target thread
        /// if necessary.
        /// </summary>
        /// <typeparam name="T">The type of object the delegate expects as a parameter.</typeparam>
        /// <param name="action">The delegate to invoke.</param>
        /// <param name="arg1">The object to send as the first parameter to the delegate.</param>
        /// <param name="arg2">The object to send as the second parameter to the delegate.</param>
        public void BeginInvoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (this.IsMarshalRequired == false)
            {
                // already on the target thread, just invoke delegate directly
                action(arg1, arg2);
            }
            else
            {
                // marshal the delegate call to the target thread
                this._synchronizationContext.Post(delegate { action(arg1, arg2); }, null);
            }
        }

        /// <summary>
        /// Calls a delegate with three parameters asynchronously.  The call will be marshaled to the target thread
        /// if necessary.
        /// </summary>
        /// <typeparam name="T">The type of object the delegate expects as a parameter.</typeparam>
        /// <param name="action">The delegate to invoke.</param>
        /// <param name="arg1">The object to send as the first parameter to the delegate.</param>
        /// <param name="arg2">The object to send as the second parameter to the delegate.</param>
        /// <param name="arg3">The object to send as the third parameter to the delegate.</param>
        public void BeginInvoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if (this.IsMarshalRequired == false)
            {
                // already on the target thread, just invoke delegate directly
                action(arg1, arg2, arg3);
            }
            else
            {
                // marshal the delegate call to the target thread
                this._synchronizationContext.Post(delegate { action(arg1, arg2, arg3); }, null);
            }
        }

        /// <summary>
        /// Calls a delegate with four parameters asynchronously.  The call will be marshaled to the target thread
        /// if necessary.
        /// </summary>
        /// <typeparam name="T">The type of object the delegate expects as a parameter.</typeparam>
        /// <param name="action">The delegate to invoke.</param>
        /// <param name="arg1">The object to send as the first parameter to the delegate.</param>
        /// <param name="arg2">The object to send as the second parameter to the delegate.</param>
        /// <param name="arg3">The object to send as the third parameter to the delegate.</param>
        /// <param name="arg4">The object to send as the fourth parameter to the delegate.</param>
        public void BeginInvoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (this.IsMarshalRequired == false)
            {
                // already on the target thread, just invoke delegate directly
                action(arg1, arg2, arg3, arg4);
            }
            else
            {
                // marshal the delegate call to the target thread
                this._synchronizationContext.Post(delegate { action(arg1, arg2, arg3, arg4); }, null);
            }
        }

        /// <summary>
        /// Invokes a delegate on a threadpool thread
        /// </summary>
        /// <param name="action">The delegate to invoke on a threadpool thread.</param>
        public static void QueueOnThreadPoolThread(Action action)
        {
            ThreadPool.QueueUserWorkItem(delegate { action(); });
        }

        /// <summary>
        /// Invokes a delegate and parameter on a threadpool thread in a typesafe way
        /// </summary>
        /// <typeparam name="T">The type of object the delegate expects as a parameter.</typeparam>
        /// <param name="action">The delegate to invoke.</param>
        /// <param name="arg">The object to send as a parameter to the delegate.</param>
        public static void QueueOnThreadPoolThread<T>(Action<T> action, T arg)
        {
            ThreadPool.QueueUserWorkItem(delegate { action(arg); });
        }

        /// <summary>
        /// Invokes a delegate with two parameters on a threadpool thread in a typesafe way
        /// </summary>
        /// <typeparam name="T">The type of object the delegate expects as a parameter.</typeparam>
        /// <param name="action">The delegate to invoke.</param>
        /// <param name="arg1">The object to send as the first parameter to the delegate.</param>
        /// <param name="arg2">The object to send as the second parameter to the delegate.</param>
        public static void QueueOnThreadPoolThread<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            ThreadPool.QueueUserWorkItem(delegate { action(arg1, arg2); });
        }

        /// <summary>
        /// Invokes a delegate with three parameters on a threadpool thread in a typesafe way
        /// </summary>
        /// <typeparam name="T">The type of object the delegate expects as a parameter.</typeparam>
        /// <param name="action">The delegate to invoke.</param>
        /// <param name="arg1">The object to send as the first parameter to the delegate.</param>
        /// <param name="arg2">The object to send as the second parameter to the delegate.</param>
        /// <param name="arg3">The object to send as the third parameter to the delegate.</param>
        public static void QueueOnThreadPoolThread<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            ThreadPool.QueueUserWorkItem(delegate { action(arg1, arg2, arg3); });
        }

        /// <summary>
        /// Invokes a delegate with four parameters on a threadpool thread in a typesafe way
        /// </summary>
        /// <typeparam name="T">The type of object the delegate expects as a parameter.</typeparam>
        /// <param name="action">The delegate to invoke.</param>
        /// <param name="arg1">The object to send as the first parameter to the delegate.</param>
        /// <param name="arg2">The object to send as the second parameter to the delegate.</param>
        /// <param name="arg3">The object to send as the second parameter to the delegate.</param>
        /// <param name="arg4">The object to send as the second parameter to the delegate.</param>
        public static void QueueOnThreadPoolThread<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ThreadPool.QueueUserWorkItem(delegate { action(arg1, arg2, arg3, arg4); });
        }
    }
}
