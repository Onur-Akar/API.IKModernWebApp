﻿using API.IKModernWebApp.Helpers.AntiCaptchaHelper.Helper;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace API.IKModernWebApp.Helpers.AntiCaptchaHelper.ApiResponse
{
    public class TaskResultResponse
    {
        public enum StatusType
        {
            Processing,
            Ready
        }
        public TaskResultResponse(dynamic json)
        {
            ErrorId = JsonHelper.ExtractInt(json, "errorId");

            if (ErrorId != null)
                if (ErrorId.Equals(0))
                {
                    Status = ParseStatus(JsonHelper.ExtractStr(json, "status"));

                    if (Status.Equals(StatusType.Ready))
                    {
                        Cost = JsonHelper.ExtractDouble(json, "cost");
                        Ip = JsonHelper.ExtractStr(json, "ip", null, true);
                        SolveCount = JsonHelper.ExtractInt(json, "solveCount", null, true);
                        CreateTime = UnixTimeStampToDateTime(JsonHelper.ExtractDouble(json, "createTime"));
                        EndTime = UnixTimeStampToDateTime(JsonHelper.ExtractDouble(json, "endTime"));

                        Solution = new SolutionData
                        {
                            Token = JsonHelper.ExtractStr(json, "solution", "token", true),
                            GRecaptchaResponse =
                                JsonHelper.ExtractStr(json, "solution", "gRecaptchaResponse", silent: true),
                            GRecaptchaResponseMd5 =
                                JsonHelper.ExtractStr(json, "solution", "gRecaptchaResponseMd5", silent: true),
                            Text = JsonHelper.ExtractStr(json, "solution", "text", silent: true),
                            Url = JsonHelper.ExtractStr(json, "solution", "url", silent: true)
                        };

                        try
                        {
                            Solution.Answers = json.solution.answers;
                        }
                        catch
                        {
                            Solution.Answers = null;
                        }

                        if (Solution.GRecaptchaResponse == null && Solution.Text == null && Solution.Answers == null
                            && Solution.Token == null)
                            DebugHelper.Out("Got no 'solution' field from API", DebugHelper.Type.Error);
                    }
                }
                else
                {
                    ErrorCode = JsonHelper.ExtractStr(json, "errorCode");
                    ErrorDescription = JsonHelper.ExtractStr(json, "errorDescription") ?? "(no error description)";

                    DebugHelper.Out(ErrorDescription, DebugHelper.Type.Error);
                }
            else
                DebugHelper.Out("Unknown error", DebugHelper.Type.Error);
        }

        public int? ErrorId { get; }
        public string ErrorCode { get; private set; }
        public string ErrorDescription { get; }
        public StatusType? Status { get; set; }
        public SolutionData Solution { get; set; }
        public double? Cost { get; set; }
        public string Ip { get; set; }

        /// <summary>
        ///     Task create time in UTC
        /// </summary>
        public DateTime? CreateTime { get; private set; }

        /// <summary>
        ///     Task end time in UTC
        /// </summary>
        public DateTime? EndTime { get; private set; }

        public int? SolveCount { get; private set; }

        private StatusType? ParseStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return null;

            try
            {
                return (StatusType)Enum.Parse(typeof(StatusType), CultureInfo.CurrentCulture.TextInfo.ToTitleCase(status), true);
            }
            catch
            {
                return null;
            }
        }

        private static DateTime? UnixTimeStampToDateTime(double? unixTimeStamp)
        {
            if (unixTimeStamp == null)
                return null;

            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            return dtDateTime.AddSeconds((double)unixTimeStamp).ToUniversalTime();
        }

        public class SolutionData
        {
            // Will be available for CustomCaptcha tasks only!
            public JObject Answers { get; internal set; }

            // Will be available for Recaptcha tasks only!
            public string GRecaptchaResponse { get; internal set; }

            // for Recaptcha with isExtended=true property
            public string GRecaptchaResponseMd5 { get; internal set; }

            // Will be available for ImageToText tasks only!
            public string Text { get; internal set; }

            public string Url { get; internal set; }

            //Will be available for FunCaptcha tasks only
            public string Token { get; internal set; }
        }
    }
}
