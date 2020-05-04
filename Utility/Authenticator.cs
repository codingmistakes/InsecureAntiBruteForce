namespace CaptchaReplay.Utility
{
    public class Authenticator
    {
        public static bool Authenticate(string username, string password)
        {
            if (username == "john" && password == "passW0rd")
            {
                return true;
            }

            return false;
        }

        public static bool CheckCaptcha(string captcha)
        {
            if (captcha == "enertioc")
            {
                return true;
            }
            return false;
        }
    }
}
