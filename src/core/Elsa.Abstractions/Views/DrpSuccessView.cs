namespace Elsa.Abstractions.DevPortal.Views
{
    public class DrpSuccessView
    {
        public DrpSuccessView()
        {
            Message = string.Empty;
        }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
    }
}
