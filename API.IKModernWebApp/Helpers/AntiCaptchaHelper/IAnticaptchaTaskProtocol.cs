using API.IKModernWebApp.Helpers.AntiCaptchaHelper.ApiResponse;
using Newtonsoft.Json.Linq;

namespace API.IKModernWebApp.Helpers.AntiCaptchaHelper
{
    public interface IAnticaptchaTaskProtocol
    {
        JObject GetPostData();
        TaskResultResponse.SolutionData GetTaskSolution();
    }
}
