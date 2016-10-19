using System;
using System.Linq;
using System.Reflection;

namespace ARSnovaPPIntegration.Communication.CastHelpers.Converters
{
    public class ObjectMapper<TSourceType, TTargetType>
    {
        private PropertyInfo[] sourceProperties;
        private PropertyInfo[] targetProperties;

        public void Map(TSourceType source, TTargetType target)
        {
            var tSource = typeof(TSourceType);
            var tTarget = typeof(TTargetType);

            this.sourceProperties = tSource.GetProperties();
            this.targetProperties = tTarget.GetProperties();

            this.SyncProperties(source, target);
        }

        private void SyncProperties(TSourceType objSource, TTargetType objTarget)
        {
            if (this.sourceProperties != null && this.sourceProperties.Any())
            {
                foreach (var sourceProperty in this.sourceProperties)
                {
                    var targetProperty = this.targetProperties.FirstOrDefault(x => x.Name == sourceProperty.Name);

                    if (targetProperty != null)
                    {
                        var val = sourceProperty.GetValue(objSource, null);
                        targetProperty.SetValue(objTarget, Convert.ChangeType(val, sourceProperty.PropertyType), null);
                    }
                }
            }
        }
    }
}
