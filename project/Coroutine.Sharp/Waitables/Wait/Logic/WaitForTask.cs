using System.Threading.Tasks;

namespace Coroutines
{

    internal class WaitForTask : WaitableTask
    {

        public WaitForTask(Task task)
        {
            task.GetAwaiter().UnsafeOnCompleted(() =>
            {
                if (task.Exception == null)
                {
                    Success();
                }
                else
                {
                    Fail(task.Exception);
                }
            });
        }

    }

    internal class WaitForTask<T> : WaitableTask<T>
    {

        public WaitForTask(Task<T> task)
        {
            task.GetAwaiter().UnsafeOnCompleted(() =>
            {
                if (task.Exception == null)
                {
                    Success(task.Result);
                }
                else
                {
                    Fail(task.Exception);
                }
            });
        }

    }

}
