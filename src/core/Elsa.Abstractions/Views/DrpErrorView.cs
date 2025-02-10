namespace Elsa.Abstractions.DevPortal.Views
{
    public class DrpErrorView
    {
        public DrpErrorView()
        {
            Message = string.Empty;
            InstanceId = "";
        }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
        public string InstanceId { get; set; }
    }
}
