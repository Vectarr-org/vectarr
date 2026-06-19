using System.Linq.Expressions;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class BlogSectionRepository : BaseRepository<BlogSection, BlogSectionModel>, IBlogSectionRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public BlogSectionRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override Expression<Func<BlogSection, BlogSectionModel>> GetEntityToModelExpression()
        {
            return entity => new BlogSectionModel()
            {
                Id = entity.Id,
                BlogId = entity.BlogId,
                Heading = entity.Heading,
                Content = entity.Content,
                Order = entity.Order,
            };
        }

        protected override BlogSection ModelToEntityInternal(BlogSectionModel model, BlogSection entity)
        {
            entity.BlogId = model.BlogId;
            entity.Heading = model.Heading;
            entity.Content = model.Content;
            entity.Order = model.Order;

            return entity;
        }
    }
}
