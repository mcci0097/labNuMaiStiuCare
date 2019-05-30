namespace lab2_restapi_1205_taskmgmt.ViewModels
{
    public class CommentGetModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool Important { get; set; }
        public int TaskId { get; set; }
    }
}