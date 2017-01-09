using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
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
            try
            {
                return GetBoolDocumentProperty("introSlideAdded");
            }
            catch (ArgumentException argumentException)
            {
                return false;
            }
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
            var slideSessionModelString = GetStringDocumentProperty("slideSessionModel");

            if (slideSessionModelString == null)
                return null;

            return JsonConvert.DeserializeObject<SlideSessionModel>(slideSessionModelString);
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

        private static string GetStringDocumentProperty(string propertyName)
        {
            /* limit by 255 chars - new solution!
             * var customProperties = GetCustomDocumentProperties();

            foreach (var property in customProperties)
            {
                if (property.Name == propertyName)
                {
                    return property.Value.ToString();
                }
            }

            return null;*/

            foreach(CustomXMLPart eachPart in Globals.ThisAddIn.Application.ActivePresentation.CustomXMLParts)
            {
                if (eachPart.DocumentElement.BaseName == propertyName)
                {
                    var xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(eachPart.XML);
                    return xmlDocument.InnerText;
                }
            }

            return null;
        }

        private static void SetStringDocumentProperty(string propertyName, string propertyValue)
        {
            /* limit by 255 chars - new solution!
             * var customProperties = GetCustomDocumentProperties();

            if (GetStringDocumentProperty(propertyName) != null)
            {
                customProperties[propertyName].Delete();
            }

            customProperties.Add(propertyName, false, MsoDocProperties.msoPropertyTypeString, propertyValue);*/
            var slideSessionModelXmlStringBuilder = new StringBuilder();

            var xmlSerializer = new XmlSerializer(typeof(XmlElement));
            var xmlElement = new XmlDocument().CreateElement(propertyName);
            xmlElement.InnerText = propertyValue;
            var writer = new StringWriter(slideSessionModelXmlStringBuilder);
            xmlSerializer.Serialize(writer, xmlElement);
            writer.Close();

            foreach (CustomXMLPart eachPart in Globals.ThisAddIn.Application.ActivePresentation.CustomXMLParts)
            {
                if (eachPart.DocumentElement.BaseName == propertyName)
                {
                    eachPart.Delete();
                    break;
                }
            }

            Globals.ThisAddIn.Application.ActivePresentation.CustomXMLParts.Add(slideSessionModelXmlStringBuilder.ToString());

            Globals.ThisAddIn.Application.ActivePresentation.Saved = MsoTriState.msoFalse; // do I really need this? 
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
