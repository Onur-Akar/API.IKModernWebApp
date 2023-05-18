using API.IKModernWebApp.Helpers.AntiCaptchaHelper.Api;

namespace API.IKModernWebApp.Helpers.AntiCaptchaHelper
{
    public static class AntiCaptchaUtils
    {
        public static bool SolveImage(string clientKey, string image, out string code)
        {
            var api = new ImageToText();
            api.ClientKey = clientKey;
            api.FilePath = image;
            code = string.Empty;
            if (!api.CreateTask())
                return false;
            if (!api.WaitForResult())
                return false;

            code = api.GetTaskSolution().Text;

            return true;
        }

        public static bool GetBalance(string clientKey, out double balance)
        {
            var api = new ImageToText();
            api.ClientKey = clientKey;
            var res = api.GetBalance();
            balance = 0;
            if (res == null)
                return false;

            balance = res.Value;
            return true;
        }
    }
}
