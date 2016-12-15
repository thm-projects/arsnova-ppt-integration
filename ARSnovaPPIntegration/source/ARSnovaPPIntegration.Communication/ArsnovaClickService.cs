using System.Collections.Generic;
using System.Linq;

using ARSnovaPPIntegration.Communication.CastHelpers.Converters;
using ARSnovaPPIntegration.Communication.CastHelpers.Models;
using ARSnovaPPIntegration.Communication.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Communication.Model.ArsnovaClick;

namespace ARSnovaPPIntegration.Communication
{
    public class ArsnovaClickService : IArsnovaClickService
    {
        private ArsnovaClickApi arsnovaClickApi;

        private readonly ObjectMapper<AnswerOptionModelWithId, AnswerOptionModel> answerOptionMapper;

        private readonly ObjectMapper<SessionConfigurationWithId, SessionConfiguration> sessionConfigurationMapper;

        public ArsnovaClickService()
        {
            this.answerOptionMapper = new ObjectMapper<AnswerOptionModelWithId, AnswerOptionModel>();

            this.sessionConfigurationMapper = new ObjectMapper<SessionConfigurationWithId, SessionConfiguration>();

            string apiUrl;

            #if DEBUG
                apiUrl = "http://localhost:3000/api/";
            #else
                apiUrl = "https://arsnova.click/api/";
            #endif

            this.arsnovaClickApi = new ArsnovaClickApi(
                apiUrl);
        }

        public List<HashtagInfo> GetAllHashtagInfos()
        {
            return this.arsnovaClickApi.GetAllHashtagInfos();
        }

        public List<AnswerOptionModel> GetAnswerOptionsForHashtag(string hashtag)
        {
            var answerOptionsReturnModel = this.arsnovaClickApi.GetAnswerOptions(hashtag);

            var answerOptions = new List<AnswerOptionModel>();

            foreach (var answerOptionModelWithId in answerOptionsReturnModel.answeroptions)
            {
                var answerOptionModel = new AnswerOptionModel();
                this.answerOptionMapper.Map(answerOptionModelWithId, answerOptionModel);
                answerOptions.Add(answerOptionModel);
            }

            return answerOptions;
        }

        public SessionConfiguration GetSessionConfiguration(string hashtag)
        {
            var sessionConfigurationReturnModel = this.arsnovaClickApi.GetSessionConfiguration(hashtag);

            var sessionConfiguration = new SessionConfiguration();

            this.sessionConfigurationMapper.Map(sessionConfigurationReturnModel.sessionConfiguration.FirstOrDefault(), sessionConfiguration);

            return sessionConfiguration;
        }

        public ValidationResult PostSession(SlideSessionModel slideSessionModel)
        {
            // Temporary: One private key per question
            slideSessionModel.PrivateKey = this.arsnovaClickApi.NewPrivateKey();

            var validationResult = this.arsnovaClickApi.AddHashtag(slideSessionModel.Hashtag, slideSessionModel.PrivateKey);

            if (!validationResult.Success)
            {
                return validationResult;
            }

            return validationResult;
            // TODO addQuestionGroup
        }

        public ValidationResult UpdateSession(SlideSessionModel slideSessionModel)
        {
            // delete existing questiongroupcollection
            // get all questions

            // add all given
            var validationResult = this.arsnovaClickApi.DeleteQuestionGroup(slideSessionModel.Hashtag, slideSessionModel.PrivateKey);

            if (!validationResult.Success)
            {
                return validationResult;
            }
            // TODO
            return validationResult;
            //return this.arsnovaClickApi.AddQuestionGroup();
        }
    }
}
