using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Classes.Permissions;
using Vectarr.Palooza.Context;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.DataAccess.Repositories;
using Vectarr.Palooza.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Vectarr.Palooza.DataAccess.IRepositories;
using Vectarr.Palooza.DataAccess.Repositories;

namespace Vectarr.Palooza.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        public int ApplicationId { get { return Application?.Id ?? 0; } }
        private VectarrPostgresDBContext _context;
        public ApplicationModel Application { get; protected set; }
        public Identity Identity { get; protected set; }
        public UserAccount CurrentUser { get; protected set; }
        protected RepositoriesContainer SiteRepositories;
        public ApplicationSubscriptionModel Subscription { get; protected set; }
        public WorkshopModel Workshop { get; protected set; }
        public CreatorModel Creator { get; protected set; }
        public UserPermissions UserPermissions { get; set; }
        private readonly RoleManager<Model.IdentityRole> _roleManager;
        private readonly UserManager<Model.UserAccount> _userManager;
        public RolesEnum Role { get; protected set; }

        public UnitOfWork(VectarrPostgresDBContext context, RoleManager<Model.IdentityRole> roleManager, UserManager<Model.UserAccount> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public UnitOfWork()
        {
            _context = CreateDbContext();
        }

        public void SetEmail(string email)
        {
            var user = GetGenericRepository<UserAccount>()
                        .GetQueryable()
                        .FirstOrDefault(w => w.Email == email);

            Model.IdentityRole role = null;

            if (user != null)
            {
                if (_userManager != null)
                {
                    var roles = _userManager.GetRolesAsync(user).Result;
                    if (roles != null && roles.Any())
                    {
                        var roleName = roles.First();
                        if (_roleManager != null)
                        {
                            role = _roleManager.FindByNameAsync(roleName).Result;
                            Role = role.Role;
                        }
                    }
                }

                var userModel = Repositories.UserAccountRepository.GetById(user.Id, skipApplicationCheck: true);
                CurrentUser = user;
                Application = GetApplication(userModel.ApplicationId);
                Identity = Repositories.IdentityRepository.GetEntityById(userModel.IdentityId, skipApplicationCheck: true);
                Subscription = Repositories.ApplicationSubscriptionRepository.GetSubscription();

                if (role != null && role.Role == RolesEnum.Workshop && userModel.WorkshopId.HasValue)
                {
                    Workshop = Repositories.WorkshopRepository.GetById(userModel.WorkshopId.Value, skipApplicationCheck: true);
                }
                else if (role != null && role.Role == RolesEnum.Creator && userModel.CreatorId.HasValue)
                {
                    Creator = Repositories.CreatorRepository.GetById(userModel.CreatorId.Value, skipApplicationCheck: true);
                }
                else if (role != null && role.Role == RolesEnum.Admin)
                {
                    // Your admin-specific logic here
                }
            }
        }

        protected ApplicationModel GetApplication(int applicationId)
        {
            return Repositories.ApplicationRepository.GetById(applicationId, skipApplicationCheck: true);
        }

        public IRepositoriesContainer Repositories
        {
            get { return SiteRepositories ?? (SiteRepositories = new RepositoriesContainer(this)); }
        }

        public IGenericRepository<T> GetGenericRepository<T>() where T : class, IBaseModel, new()
        {
            return new GenericRepository<T>(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public VectarrPostgresDBContext CreateDbContext()
        {
            IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();

            var builder = new DbContextOptionsBuilder<VectarrPostgresDBContext>();
            var connectionString =
                configuration.GetConnectionString("ProdDefaultConnection");
            builder.UseNpgsql(connectionString);

            return new VectarrPostgresDBContext(builder.Options);
        }
    }
}
