using System;
using System.Reflection;

using Microsoft.Office.Core;

namespace ARSnovaPPIntegration.Presentation.Helpers
{
    public static class PresentationPropertiesAccessor
    {
        public static void SetStringDocumentProperty(string propertyName, string propertyValue)
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

        public static void SetBoolDocumentProperty(string propertyName, bool propertyValue)
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

        public static bool HasDocumentProperty(string propertyName)
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

        public static object GetDocumentProperty(string propertyName, MsoDocProperties type)
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
