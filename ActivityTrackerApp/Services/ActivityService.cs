using AutoMapper;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// Activity service.
    /// </summary>
    public class ActivityService : IActivityService
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public ActivityService(IDataContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
    }
}