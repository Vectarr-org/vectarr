using System.Linq.Expressions;
using LinqKit;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class BlogRepository : BaseRepository<Blog, BlogModel>, IBlogRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public BlogRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override Expression<Func<Blog, BlogModel>> GetEntityToModelExpression()
        {
            var imageExpression = _unitOfWork.Repositories.ImageRepository.GetEntityToModelExpression().Expand();
            var sectionExpression = _unitOfWork.Repositories.BlogSectionRepository.GetEntityToModelExpression().Expand();
            var keywordExpression = _unitOfWork.Repositories.BlogKeywordRepository.GetEntityToModelExpression().Expand();
            return entity => new BlogModel()
            {
                Id = entity.Id,
                Slug = entity.Slug,
                Title = entity.Title,
                Excerpt = entity.Excerpt,
                Description = entity.Description,
                Image = entity.Image != null ? imageExpression.Invoke(entity.Image) : null,
                ImageId = entity.ImageId,
                Category = entity.Category,
                ReadTimeMinutes = entity.ReadTimeMinutes,
                PublishedAt = entity.PublishedAt,
                Sections = entity.Sections.AsQueryable().Select(s => sectionExpression.Invoke(s)).ToList(),
                Keywords = entity.Keywords.AsQueryable().Select(k => keywordExpression.Invoke(k)).ToList(),
            };
        }

        protected override Blog ModelToEntityInternal(BlogModel model, Blog entity)
        {
            entity.Slug = model.Slug;
            entity.Title = model.Title;
            entity.Excerpt = model.Excerpt;
            entity.Description = model.Description;
            entity.ImageId = model.ImageId;
            entity.Category = model.Category;
            entity.ReadTimeMinutes = model.ReadTimeMinutes;
            entity.PublishedAt = model.PublishedAt;

            var image = _unitOfWork.Repositories.ImageRepository.GetEntityById(model.ImageId);
            entity.Image = image;

            return entity;
        }
    }
}
