using System.Linq.Expressions;
using LinqKit;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.DataAccess.IInfrastructure.DeleteHandler
{
    public class DeleteHandler<TEntity> where TEntity : class, IBaseModel, new()
    {
        private readonly IRepositoryProxy<TEntity> _proxy;
        private readonly Action _registerDependenciesAction;
        private readonly List<Action<TEntity, DeleteModeEnum>> _tasksToExecuteBeforeDelete = new List<Action<TEntity, DeleteModeEnum>>();
        private readonly List<Action<TEntity, DeleteModeEnum>> _tasksToExecuteAfterDelete = new List<Action<TEntity, DeleteModeEnum>>();
        private bool _dependenciesRegistered;

        internal DeleteHandler(IRepositoryProxy<TEntity> proxy, Action registerDependenciesAction)
        {
            _proxy = proxy;
            _registerDependenciesAction = registerDependenciesAction;
        }

        public void Delete(TEntity entity, DeleteModeEnum deleteMode)
        {
            if (!_dependenciesRegistered)
            {
                _registerDependenciesAction();
                _dependenciesRegistered = true;
            }

            ExecuteBeforeDeleteDependencies(entity, deleteMode);

            switch (deleteMode)
            {
                case DeleteModeEnum.HardDelete:
                    _proxy.Delete(entity);
                    break;
                case DeleteModeEnum.SoftDelete:
                    entity.IsRemoved = true;
                    _proxy.Update(entity);
                    break;
                case DeleteModeEnum.Hide:
                    entity.IsHidden = true;
                    _proxy.Update(entity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(deleteMode), deleteMode, null);
            }

            ExecuteAfterDeleteDependencies(entity, deleteMode);
        }

        public void Delete(List<TEntity> entities, DeleteModeEnum deleteMode)
        {
            if (!_dependenciesRegistered)
            {
                _registerDependenciesAction();
                _dependenciesRegistered = true;
            }

            entities.ForEach(x => { ExecuteBeforeDeleteDependencies(x, deleteMode); });

            switch (deleteMode)
            {
                case DeleteModeEnum.HardDelete:
                    _proxy.Delete(entities);
                    break;
                case DeleteModeEnum.SoftDelete:
                    entities.ForEach(x => x.IsRemoved = true);
                    _proxy.Update(entities);
                    break;
                case DeleteModeEnum.Hide:
                    entities.ForEach(x => x.IsHidden = true);
                    _proxy.Update(entities);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(deleteMode), deleteMode, null);
            }

            entities.ForEach(x => { ExecuteAfterDeleteDependencies(x, deleteMode); });
        }

        public void RegisterDependentProperty<TSubEntity>(Expression<Func<TEntity, TSubEntity>> selector, DeleteHandler<TSubEntity> deleteHandler)
            where TSubEntity : class, IBaseModel, new()
        {
            _tasksToExecuteBeforeDelete.Add((entity, deleteMode) =>
            {
                var subEntity = selector.Invoke(entity);
                if (subEntity != null)
                {
                    if (deleteMode == DeleteModeEnum.HardDelete ||
                    deleteMode == DeleteModeEnum.SoftDelete && subEntity.IsRemoved == false
                    || deleteMode == DeleteModeEnum.Hide && subEntity.IsHidden == false)
                    {
                        deleteHandler.Delete(subEntity, deleteMode);
                    }
                }
            });
        }

        public void RegisterDependentCollection<TSubEntity>(Expression<Func<TEntity, ICollection<TSubEntity>>> selector, DeleteHandler<TSubEntity> deleteHandler)
            where TSubEntity : class, IBaseModel, new()
        {
            _tasksToExecuteBeforeDelete.Add((entity, deleteMode) =>
            {
                var subEntities = selector.Invoke(entity);
                if (subEntities != null)
                {
                    var list = subEntities.ToList();
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        var subEntity = list[i];
                        if (deleteMode == DeleteModeEnum.SoftDelete && subEntity.IsRemoved
                          || deleteMode == DeleteModeEnum.Hide && subEntity.IsHidden)
                        {
                            list.RemoveAt(i);
                        }
                    }
                    deleteHandler.Delete(list, deleteMode);
                }
            });
        }

        public void RegisterBeforeDeleteAction(Action<TEntity, DeleteModeEnum> action)
        {
            _tasksToExecuteBeforeDelete.Add(action);
        }

        public void RegisterAfterDeleteAction(Action<TEntity, DeleteModeEnum> action)
        {
            _tasksToExecuteAfterDelete.Add(action);
        }

        public void ExecuteBeforeDeleteDependencies(TEntity entity, DeleteModeEnum deleteMode)
        {
            foreach (var action in _tasksToExecuteBeforeDelete)
            {
                action.Invoke(entity, deleteMode);
            }
        }

        public void ExecuteAfterDeleteDependencies(TEntity entity, DeleteModeEnum deleteMode)
        {
            foreach (var action in _tasksToExecuteAfterDelete)
            {
                action.Invoke(entity, deleteMode);
            }
        }
    }
}
