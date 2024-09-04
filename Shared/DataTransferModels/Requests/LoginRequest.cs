namespace DataTransferModels.Requests
{
    public class LoginRequest(string userName)
    {
        public string UserName { get; set; } = userName;
    }
}
