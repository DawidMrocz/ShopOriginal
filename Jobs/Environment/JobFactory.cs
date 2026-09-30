using Jobs.Enums;
using Jobs.Extensions;
using Jobs.Infrastructure.Attributes;
using Jobs.Job;
using log4net;

namespace Jobs.Environment
{
    public class JobFactory
    {
        private readonly ILog _logger;

        public JobFactory()
        {
            _logger = LogManager.GetLogger(typeof(JobFactory));
        }

        public IJob Create(JobTypeEnum jobType, IServiceProvider serviceProvider)
        {

            IJob job = null;
            try
            {
                var service = GetFromEnum(jobType, serviceProvider);
                if (service == null || jobType == JobTypeEnum.Undefined)
                    throw new ArgumentException("Wrong Value", "jobType");
                return service;
            }
            catch (Exception ex)
            {
                _logger.Error("Błąd podczas tworzenia joba", ex);
                Console.WriteLine(ex);
            }
            return job;
        }

        private IJob GetFromEnum(JobTypeEnum jobType, IServiceProvider serviceProvider)
        {
            var typeName = jobType.GetAttribute<ImplementationJobAttribute>()?.ServiceName ?? jobType.ToString();

            var types = JobTypes().Where(x => x.Name == typeName);
            if (types == null || !types.Any())
                throw new Exception("Lack Implementation Job");
            if (types.Count() != 1)
                throw new Exception("Exists more than 1 implementation Job");
            return (IJob)Activator.CreateInstance(types.First(), serviceProvider);
        }

        private List<Type> JobTypes()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                 .Where(mytype => !mytype.IsAbstract && mytype.GetInterfaces().Contains(typeof(IJob))).ToList();

        }
    }
}
