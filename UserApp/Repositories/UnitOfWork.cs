using Framework.Repositories.Currency;
using Framework.Repositories.File;
using Framework.Repositories.User;
using Framework.Services.Email;
using Framework.Services.User;
using MassTransit;
using UserApp.Data;
using UserApp.Services.Favorite;

namespace UserApp.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private UserContext _context;
        private UserRepository<UserContext, Models.User, Guid> _userRepository;
        private FileRepository<UserContext> _fileRepository;
        private UserService<Models.User, Guid> _userService;
        private IEmailService _emailService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ICurrencyRepository _currencyRepository;
        private FavoriteService _favoriteService;
        private readonly ProductAppGrpc.Product.ProductClient _productClient;


        public UnitOfWork(IConfiguration configuration, IEmailService emailService, IPublishEndpoint publishEndpoint, ICurrencyRepository currencyRepository, ProductAppGrpc.Product.ProductClient productClient)
        {
            _context = new UserContext(configuration);
            _emailService = emailService;
            _publishEndpoint = publishEndpoint;
            _currencyRepository = currencyRepository;
            _fileRepository = new FileRepository<UserContext>(_context);         
            _productClient = productClient;
            _favoriteService = new FavoriteService(_context, publishEndpoint, currencyRepository, _productClient);
        }

        public UserRepository<UserContext, Models.User, Guid> UserRepository => _userRepository ?? new UserRepository<UserContext, Models.User, Guid>(_context);
        public FileRepository<UserContext> FileRepository => _fileRepository;
        public UserService<Models.User, Guid> UserService => _userService ?? new UserService<Models.User, Guid>(UserRepository, _emailService, FileRepository);
        public FavoriteService FavoriteService => _favoriteService ?? new FavoriteService(_context, _publishEndpoint, _currencyRepository, _productClient);

        public Task Save() => _context.SaveChangesAsync();

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
                if (disposing)
                    _context.Dispose();

            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
