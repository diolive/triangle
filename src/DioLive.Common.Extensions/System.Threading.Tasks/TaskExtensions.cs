namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static void Forget(this Task task)
        {
        }

        public static void AsSync(this Task task)
        {
            task.GetAwaiter().GetResult();
        }
    }
}