namespace CircleIdleLib
{
    public class CompletedTasks
    {
        public int Completed;
        public string ItemName;
        public string CompletedBy;
        public string TaskType;
        public CompletedTasks(int completedQty, string whatIsCompleted, string whoCompleted, string type)
        {
            Completed = completedQty;
            ItemName = whatIsCompleted;
            CompletedBy = whoCompleted;
            TaskType = type;
        }
    }
}