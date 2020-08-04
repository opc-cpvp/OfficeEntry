using Microsoft.JSInterop;

namespace OfficeEntry.WebApp
{
    public interface ISurveyInterop
    {
        void CreateSurvey(string id);
    }

    public class SurveyInterop : ISurveyInterop
    {
        private readonly IJSRuntime _jsRuntime;

        public SurveyInterop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public void CreateSurvey(string id)
        {
            _jsRuntime.InvokeAsync<string>("interopJS.survey.init", id);
        }
    }
}