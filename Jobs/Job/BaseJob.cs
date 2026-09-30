namespace Jobs.Job
{
    public abstract class BaseJob : IJob
    {
        protected readonly IServiceProvider _serviceProvider;

        protected readonly string JobType;
        private readonly string _startJobMsg;
        private readonly string _endJobMsg;
        private readonly string _errorJobMsg;

        protected BaseJob(IServiceProvider serviceProvider)
        {
            JobType = GetType().Name;
            _serviceProvider = serviceProvider;
            _startJobMsg = string.Format("Start {0}", JobType);
            _endJobMsg = string.Format("End {0}", JobType);
            _errorJobMsg = string.Format("Wystąpił błąd przy wywołaniu Joba: {0}", JobType);
        }

        public async Task Run()
        {
            try
            {
                await JobRun();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// W tej metodzie umieszczaj swoje wywołania serwisów potrzebnych do osbługi Joba
        /// </summary>
        public abstract Task JobRun();
    }
}
