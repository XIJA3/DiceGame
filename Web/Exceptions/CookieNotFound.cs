namespace Web.Exceptions
{
    public class CookieNotFound(string message = "Can't Find Cookie") : CustomException(message) { }
}
