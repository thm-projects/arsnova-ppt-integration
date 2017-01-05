using System;

using Newtonsoft.Json;

using Microsoft.Office.Core;

using ARSnovaPPIntegration.Business.Model;

namespace ARSnovaPPIntegration.Presentation.Helpers
{
    public static class PresentationInformationStore
    {
        /* currently used params to store informations:
            - introSlideAdded (bool)
            - stringified slideSessionModel (string)
        */

        public static void SetArsnovaIntroSlideAdded()
        {
            SetBoolDocumentProperty("introSlideAdded", true);
        }

        public static bool IsArsnovaIntroSlideAlreadyAdded()
        {
            return GetBoolDocumentProperty("introSlideAdded");
        }

        public static void StoreSlideSessionModel(SlideSessionModel slideSessionModel)
        {
            var slideSessionModelString = JsonConvert.SerializeObject(slideSessionModel);
            SetStringDocumentProperty("slideSessionModel", slideSessionModelString);
        }

        public static bool HasSlideSessionModel()
        {
            return GetStringDocumentProperty("slideSessionModel") != null;
        }

        public static SlideSessionModel GetStoredSlideSessionModel()
        {
            if (GetStringDocumentProperty("slideSessionModel") == null)
                return null;

            var slideSessionModelString = GetStringDocumentProperty("slideSessionModel");

            return JsonConvert.DeserializeObject<SlideSessionModel>(slideSessionModelString);
        }

        private static void SetStringDocumentProperty(string propertyName, string propertyValue)
        {
            var customProperties = GetCustomDocumentProperties();

            if (GetStringDocumentProperty(propertyName) != null)
            {
                customProperties[propertyName].Delete();
            }

            customProperties.Add(propertyName, false, MsoDocProperties.msoPropertyTypeString, propertyValue);
        }

        private static void SetBoolDocumentProperty(string propertyName, bool propertyValue)
        {
            var customProperties = GetCustomDocumentProperties();

            if (HasBoolDocumentProperty(propertyName))
            {
                customProperties[propertyName].Delete();
            }

            customProperties.Add(propertyName, false, MsoDocProperties.msoPropertyTypeBoolean, propertyValue);
        }

        private static string GetStringDocumentProperty(string propertyName)
        {
            var customProperties = GetCustomDocumentProperties();

            foreach (var property in customProperties)
            {
                if (property.Name == propertyName)
                {
                    return property.Value.ToString();
                }
            }

            return null;
        }

        private static bool GetBoolDocumentProperty(string propertyName)
        {
            var customProperties = GetCustomDocumentProperties();

            foreach (var property in customProperties)
            {
                if (property.Name == propertyName)
                {
                    return property.Value;
                }
            }

            throw new ArgumentException($"Boolean property {propertyName} does not exist in current presentation.");
        }

        private static bool HasBoolDocumentProperty(string propertyName)
        {
            var customProperties = GetCustomDocumentProperties();

            foreach (var property in customProperties)
            {
                if (property.Name == propertyName)
                {
                    return true;
                }
            }

            return false;
        }

        private static dynamic GetCustomDocumentProperties()
        {
            var presentation = Globals.ThisAddIn.Application.ActivePresentation;

            if (presentation != null)
            {
                return presentation.CustomDocumentProperties;
            }
            else
            {
                throw new NullReferenceException("No active presentation");
            }
        }
    }
}
