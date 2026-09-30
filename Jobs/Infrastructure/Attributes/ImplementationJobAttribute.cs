using System;

namespace Jobs.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    class ImplementationJobAttribute : Attribute
    {
        public string ServiceName;

        /// <summary>
        /// Service namne, Inherited IJob
        /// </summary>
        /// <param name="serviceName"></param>
        public ImplementationJobAttribute(string serviceName)
        {
            ServiceName = serviceName;
        }
    }
}
