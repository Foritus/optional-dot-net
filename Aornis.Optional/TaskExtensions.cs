using System;
using System.Threading.Tasks;

namespace Aornis
{
    /// <summary>
    /// Extension methods for chaining async operations with Optional values
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Awaits the given Task and, if it returns Optional.Empty, calls the given value factory function instead
        /// </summary>
        /// <typeparam name="T">The type of value that is being stored in the Optional result of the task</typeparam>
        /// <param name="task">The task to await</param>
        /// <param name="callback">The function to call if the given task's returned value is Optional.Empty</param>
        /// <returns>The value returned by the task, or the value returned by callback if it the task returned Optional.Empty</returns>
        public static async Task<T> OrElseAsync<T>(this Task<Optional<T>> task, Func<T> callback)
        {
            return (await task).OrElse(callback);
        }
        
        /// <summary>
        /// Awaits the given Task and, if it returns Optional.Empty, returns elseValue instead
        /// </summary>
        /// <typeparam name="T">The type of value that is being stored in the Optional result of the task</typeparam>
        /// <param name="task">The task to await</param>
        /// <param name="elseValue">The value to return if the given task's returned value is Optional.Empty</param>
        /// <returns>The value returned by the task, or elseValue if it the task returned Optional.Empty</returns>
        public static async Task<T> OrElseAsync<T>(this Task<Optional<T>> task, T elseValue)
        {
            return (await task).OrElse(elseValue);
        }
        
        /// <summary>
        /// Awaits the given Task and, if it returns Optional.Empty, calls the given value factory function instead
        /// </summary>
        /// <typeparam name="T">The type of value that is being stored in the Optional result of the task</typeparam>
        /// <param name="task">The task to await</param>
        /// <param name="callback">The function to call if the given task's returned value is Optional.Empty</param>
        /// <returns>The value returned by the task, or the value returned by callback if it the task returned Optional.Empty</returns>
        public static async Task<Optional<T>> OrElseAsync<T>(this Task<Optional<T>> task, Func<Optional<T>> callback)
        {
            return (await task).OrElse(callback);
        }

        /// <summary>
        /// Awaits the given source Task and maps the resulting value using the given function
        /// </summary>
        /// <typeparam name="TResult">The type of value that is being processed by the source task</typeparam>
        /// <typeparam name="TNewValue">The type that the result of the task will be mapped to</typeparam>
        /// <param name="task">The task to await</param>
        /// <param name="callback">The function to call if the given task's returned value is not Optional.Empty</param>
        /// <returns>The value returned by the task, or the value returned by callback if it the task returned Optional.Empty</returns>
        public static Task<Optional<TNewValue>> FlatMapAsync<TResult, TNewValue>(
            this Task<Optional<TResult>> task,
            Func<TResult, Optional<TNewValue>> callback
        )
        {
            return task.ContinueWith(x => x.Result.FlatMap(callback));
        }
        
        /// <summary>
        /// Awaits the given source Task and maps the resulting value using the given function
        /// </summary>
        /// <typeparam name="TResult">The type of value that is being processed by the source task</typeparam>
        /// <typeparam name="TNewValue">The type that the result of the task will be mapped to</typeparam>
        /// <param name="task">The task to await</param>
        /// <param name="callback">The function to call if the given task's returned value is not Optional.Empty</param>
        /// <returns>The value returned by the task, or the value returned by callback if it the task returned Optional.Empty</returns>
        public static Task<Optional<TNewValue>> FlatMapAsync<TResult, TNewValue>(
            this Task<Optional<TResult>> task,
            Func<TResult, Task<Optional<TNewValue>>> callback
        )
        {
            return task.ContinueWith(x =>
            {
                return x.Result.FlatMapAsync(async value => await callback(value));
            })
            // Unwrap the Task<Task<Optional<T>>> into a Task<Optional<T>>
            .Unwrap();
        }
        
        /// <summary>
        /// Awaits the given source Task and executes the given callback function if the previous task returned a value
        /// </summary>
        /// <typeparam name="TResult">The type of value that is being processed by the source task</typeparam>
        /// <param name="task">The task to await</param>
        /// <param name="callback">The function to call if the given task's returned value is not Optional.Empty</param>
        /// <returns>The value returned by the task, or the value returned by callback if it the task returned Optional.Empty</returns>
        public static async Task<Optional<TResult>> IfPresentAsync<TResult>(
            this Task<Optional<TResult>> task,
            Action<TResult> callback
        )
        {
            var result = await task;
            return result.IfPresent(callback);
        }
        
        /// <summary>
        /// Awaits the given source Task and executes the given callback function if the previous task returned a value
        /// </summary>
        /// <typeparam name="TResult">The type of value that is being processed by the source task</typeparam>
        /// <param name="task">The task to await</param>
        /// <param name="callback">The function to call if the given task's returned value is not Optional.Empty</param>
        /// <returns>The value returned by the task, or the value returned by callback if it the task returned Optional.Empty</returns>
        public static async Task<Optional<TResult>> IfPresentAsync<TResult>(
            this Task<Optional<TResult>> task,
            Func<TResult, Task> callback
        )
        {
            var result = await task;
            return await result.IfPresentAsync(async value => await callback(value).ConfigureAwait(true));
        }

        /// <summary>
        /// Awaits the given source Task and maps the resulting value using the given function
        /// </summary>
        /// <typeparam name="TResult">The type of value that is being processed by the source task</typeparam>
        /// <typeparam name="TNewValue">The type that the result of the task will be mapped to</typeparam>
        /// <param name="task">The task to await</param>
        /// <param name="callback">The function to call if the given task's returned value is Optional.Empty</param>
        /// <returns>The value returned by the task, or the value returned by callback if it the task returned Optional.Empty</returns>
        public static Task<Optional<TNewValue>> MapAsync<TResult, TNewValue>(this Task<Optional<TResult>> task, Func<TResult, TNewValue> callback)
        {
            return task.ContinueWith(x => x.Result.Map(callback));
        }
        
        /// <summary>
        /// Awaits the given source Task and maps the resulting value using the given function
        /// </summary>
        /// <remarks>This overload is specialised to the Task (non-generic) case, to avoid async void behavioural weirdness</remarks>
        /// <typeparam name="TResult">The type of value that is being processed by the source task</typeparam>
        /// <param name="task">The task to await</param>
        /// <param name="callback">The function to call if the given task's returned value is Optional.Empty</param>
        /// <returns>The value returned by the task, or the value returned by callback if it the task returned Optional.Empty</returns>
        public static async Task MapAsync<TResult>(
            this Task<Optional<TResult>> task,
            Func<TResult, Task> callback
        )
        {
            var result = await task;
            await result.IfPresentAsync(async value => await callback(value).ConfigureAwait(true));
        }
        
        /// <summary>
        /// Awaits the given source Task and maps the resulting value using the given function
        /// </summary>
        /// <typeparam name="TResult">The type of value that is being processed by the source task</typeparam>
        /// <typeparam name="TNewValue">The type that the result of the task will be mapped to</typeparam>
        /// <param name="task">The task to await</param>
        /// <param name="callback">The function to call if the given task's returned value is Optional.Empty</param>
        /// <returns>The value returned by the task, or the value returned by callback if it the task returned Optional.Empty</returns>
        public static Task<Optional<TNewValue>> MapAsync<TResult, TNewValue>(
            this Task<Optional<TResult>> task,
            Func<TResult, Task<TNewValue>> callback
        )
        {
            return task.ContinueWith(x =>
            {
                return x.Result.MapAsync(async value => await callback(value));
            })
            // Unwrap the Task<Task<Optional<T>>> into a Task<Optional<T>>
            .Unwrap();
        }
    }
}