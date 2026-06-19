using System.Linq.Expressions;
using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.IInfrastructure;
using Vectarr.Palooza.DataAccess.IInfrastructure.DeleteHandler;

namespace Vectarr.Palooza.DataAccess.Infrastructure
{
    public abstract class BaseRepository<TEntity, TModel> : GetRepository<TEntity, TModel>, IEntityRepository<TEntity, TModel>
          where TEntity : class, IBaseModel, new()
          where TModel : class, IApiModel, new()
    {
        private DeleteHandler<TEntity> _deleteHandler;

        public DeleteHandler<TEntity> GetDeleteHandler()
        {
            return _deleteHandler ?? (_deleteHandler = new DeleteHandler<TEntity>(RepositoryProxy, RegisterDependencies));
        }

        protected BaseRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #region Update

        public virtual TModel Update(TModel model)
        {
            if (model.Id == 0)
            {
                throw new ArgumentException("model id must exist");
            }

            var entity = GetEntityById(model.Id);
            entity = ModelToEntity(model, entity);

            if (entity == null)
            {
                throw new ArgumentException("Entity does not exist in the database or model Id is incorrectly set");
            }

            entity.LastModified = DateTime.UtcNow;
            RepositoryProxy.Update(entity);

            model = GetById(model.Id);
            return model;
        }

        public virtual TModel UpdateWithInclude(TModel model, bool skipApplicationCheck = false)
        {
            if (model.Id == 0)
            {
                throw new ArgumentException("model id must exist");
            }

            var entity = GetEntityByIdWithInclude(model.Id, skipApplicationCheck);
            entity = ModelToEntity(model, entity);

            if (entity == null)
            {
                throw new ArgumentException("Entity does not exist in the database or model Id is incorrectly set");
            }

            entity.LastModified = DateTime.UtcNow;
            RepositoryProxy.Update(entity);

            model = GetById(model.Id);
            return model;
        }

        public void Update(TEntity entity)
        {
            RepositoryProxy.Update(entity);
        }

        public void Update(List<TEntity> entities)
        {
            RepositoryProxy.Update(entities);
        }

        public void Update(List<TModel> models)
        {
            List<TEntity> entities = new List<TEntity>();
            models.ForEach(model =>
            {
                var entity = GetEntityById(model.Id);
                entity = ModelToEntity(model, entity);

                if (entity == null)
                {
                    throw new ArgumentException("Entity does not exist in the database or model Id is incorrectly set");
                }
                entity.LastModified = DateTime.UtcNow;
                entities.Add(entity);
            });
            Update(entities);
        }

        #endregion

        #region Insert

        public virtual TModel Insert(TModel model)
        {
            var entity = new TEntity();

            entity = ModelToEntity(model, entity);
            entity = RepositoryProxy.Insert(entity);
            model = GetById(entity.Id, skipApplicationCheck: true);
            return model;
        }
        public TEntity Insert(TEntity entity)
        {
            entity = RepositoryProxy.Insert(entity);
            return entity;
        }
        #endregion


        public void Delete(TModel model, bool softDelete = true)
        {
            var entity = GetEntityById(model.Id);
            var deleteMode = ShouldHideInsteadOfDelete(entity) ? DeleteModeEnum.Hide : (softDelete ? DeleteModeEnum.SoftDelete : DeleteModeEnum.HardDelete);
            GetDeleteHandler().Delete(entity, deleteMode);
        }

        public void Delete(List<TModel> models, bool softDelete = true)
        {
            foreach (var model in models)
            {
                var entity = GetEntityById(model.Id);
                var deleteMode = ShouldHideInsteadOfDelete(entity) ? DeleteModeEnum.Hide : (softDelete ? DeleteModeEnum.SoftDelete : DeleteModeEnum.HardDelete);
                GetDeleteHandler().Delete(entity, deleteMode);
            }
        }

        public void Delete(TEntity entity, bool softDelete = true)
        {
            var refreshedEntity = GetEntityById(entity?.Id, skipApplicationCheck: true);

            if (refreshedEntity == null || refreshedEntity.IsRemoved)
            {
                return;
            }

            var deleteMode = ShouldHideInsteadOfDelete(entity) ? DeleteModeEnum.Hide : (softDelete ? DeleteModeEnum.SoftDelete : DeleteModeEnum.HardDelete);
            GetDeleteHandler().Delete(entity, deleteMode);
        }

        public void Delete(int id, bool softDelete = true)
        {
            var entity = GetEntityById(id);

            if (entity == null || entity.IsRemoved)
            {
                return;
            }

            var deleteMode = ShouldHideInsteadOfDelete(entity) ? DeleteModeEnum.Hide : (softDelete ? DeleteModeEnum.SoftDelete : DeleteModeEnum.HardDelete);
            GetDeleteHandler().Delete(entity, deleteMode);
        }

        public void Delete(List<TEntity> entities, bool softDelete = true)
        {
            entities.ForEach(entity =>
            {
                var deleteMode = ShouldHideInsteadOfDelete(entity) ? DeleteModeEnum.Hide : (softDelete ? DeleteModeEnum.SoftDelete : DeleteModeEnum.HardDelete);
                GetDeleteHandler().Delete(entity, deleteMode);
            });
        }

        public void Delete(List<int> ids, bool softDelete = true)
        {
            var entities = GetEntitiesById(ids);
            Delete(entities, softDelete);
        }


        public List<TEntity> ReplaceCollection(IEnumerable<TEntity> original, List<TEntity> updated, bool softDelete = true)
        {
            if (original == null || !original.Any())
            {
                return updated;
            }

            List<TEntity> result = new List<TEntity>();

            var removedEntities = original.Except(updated).ToList();
            removedEntities.AddRange(updated.Where(e => e.IsRemoved));

            if (!softDelete)
            {
                Delete(removedEntities, false);
            }
            else
            {
                removedEntities.ForEach(e =>
                {
                    if (!e.IsRemoved)
                        Delete(e);
                });
                result.AddRange(removedEntities);
            }

            result.AddRange(updated.Where(e => e.IsRemoved == false));

            return result;
        }

        /// <summary>
        /// Updates an entity from a model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity ModelToEntity(TModel model, TEntity entity)
        {
            return ModelToEntityInternal(model, entity);
        }

        /// <summary>
        /// Updates an entity from a model. overridable in each repository
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected abstract TEntity ModelToEntityInternal(TModel model, TEntity entity);

        protected void RegisterDependentProperty<TSubEntity, TSubModel>(Expression<Func<TEntity, TSubEntity>> selector, IEntityRepository<TSubEntity, TSubModel> repository)
     where TSubEntity : class, IBaseModel, new()
     where TSubModel : class, IApiModel, new()
        {
            GetDeleteHandler().RegisterDependentProperty(selector, repository.GetDeleteHandler());
        }

        protected void RegisterDependentProperty<TSubEntity, TSubModel>(Expression<Func<TEntity, ICollection<TSubEntity>>> selector, IEntityRepository<TSubEntity, TSubModel> repository)
            where TSubEntity : class, IBaseModel, new()
            where TSubModel : class, IApiModel, new()
        {
            GetDeleteHandler().RegisterDependentCollection(selector, repository.GetDeleteHandler());
        }

        protected void RegisterBeforeDeleteAction(Action<TEntity, DeleteModeEnum> action)
        {
            GetDeleteHandler().RegisterBeforeDeleteAction(action);
        }

        protected void RegisterAfterDeleteAction(Action<TEntity, DeleteModeEnum> action)
        {
            GetDeleteHandler().RegisterAfterDeleteAction(action);
        }
        protected virtual void RegisterDependencies()
        {

        }
        public virtual bool ShouldHideInsteadOfDelete(TEntity entity)
        {
            return false;
        }

    }
}
