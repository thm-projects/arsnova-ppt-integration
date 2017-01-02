using System;
using System.Reflection;

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
            return (bool)GetDocumentProperty("introSlideAdded", MsoDocProperties.msoPropertyTypeBoolean);
        }

        public static void StoreSlideSessionModel(SlideSessionModel slideSessionModel)
        {
            var slideSessionModelString = JsonConvert.SerializeObject(slideSessionModel);
            SetStringDocumentProperty("slideSessionModel", slideSessionModelString);
        }

        public static SlideSessionModel GetStoredSlideSessionModel()
        {
            if (HasDocumentProperty("slideSessionModel"))
            {
                var slideSessionModelString = (string)GetDocumentProperty("slideSessionModel", MsoDocProperties.msoPropertyTypeString);

                return JsonConvert.DeserializeObject<SlideSessionModel>(slideSessionModelString);
            }

            return null;
        }

        private static void SetStringDocumentProperty(string propertyName, string propertyValue)
        {
            object oDocCustomProps = GetCustomDocumentProperties();
            var typeDocCustomProps = oDocCustomProps.GetType();

            object[] oArgs = {propertyName,false,
                 MsoDocProperties.msoPropertyTypeString,
                 propertyValue};

            typeDocCustomProps.InvokeMember("Add", BindingFlags.Default |
                                       BindingFlags.InvokeMethod, null,
                                       oDocCustomProps, oArgs);

        }

        private static void SetBoolDocumentProperty(string propertyName, bool propertyValue)
        {
            object oDocCustomProps = GetCustomDocumentProperties();
            var typeDocCustomProps = oDocCustomProps.GetType();

            object[] oArgs = {propertyName,false,
                 MsoDocProperties.msoPropertyTypeBoolean,
                 propertyValue};

            typeDocCustomProps.InvokeMember("Add", BindingFlags.Default |
                                       BindingFlags.InvokeMethod, null,
                                       oDocCustomProps, oArgs);
        }

        private static bool HasDocumentProperty(string propertyName)
        {
            object oDocCustomProps = GetCustomDocumentProperties();
            var typeDocCustomProps = oDocCustomProps.GetType();

            try
            {
                object returned = typeDocCustomProps.InvokeMember("Item",
                                        BindingFlags.Default |
                                       BindingFlags.GetProperty, null,
                                       oDocCustomProps, new object[] { propertyName });

                return returned != null;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private static object GetDocumentProperty(string propertyName, MsoDocProperties type)
        {
            object returnVal;

            object oDocCustomProps = GetCustomDocumentProperties();
            var typeDocCustomProps = oDocCustomProps.GetType();


            object returned = typeDocCustomProps.InvokeMember("Item",
                                        BindingFlags.Default |
                                       BindingFlags.GetProperty, null,
                                       oDocCustomProps, new object[] { propertyName });

            if (returned == null)
            {
                throw new NullReferenceException($"Property with name {propertyName} not provided by active presentation.");
            }

            var typeDocAuthorProp = returned.GetType();
            returnVal = typeDocAuthorProp.InvokeMember("Value",
                                       BindingFlags.Default |
                                       BindingFlags.GetProperty,
                                       null, returned,
                                       new object[] { }).ToString();

            return returnVal;
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
