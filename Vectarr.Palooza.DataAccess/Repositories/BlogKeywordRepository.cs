using System.Linq.Expressions;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class BlogKeywordRepository : BaseRepository<BlogKeyword, BlogKeywordModel>, IBlogKeywordRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public BlogKeywordRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override Expression<Func<BlogKeyword, BlogKeywordModel>> GetEntityToModelExpression()
        {
            return entity => new BlogKeywordModel()
            {
                Id = entity.Id,
                BlogId = entity.BlogId,
                Keyword = entity.Keyword,
            };
        }

        protected override BlogKeyword ModelToEntityInternal(BlogKeywordModel model, BlogKeyword entity)
        {
            entity.BlogId = model.BlogId;
            entity.Keyword = model.Keyword;

            return entity;
        }
    }
}
