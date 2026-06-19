using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.DataAccess.Infrastructure
{
    public class PredicateGenerator<TEntity, TPropertyType> where TEntity : class, new()
    {
        public Expression<Func<TEntity, bool>> Create(IEnumerable<TPropertyType> collection, Expression<Func<TEntity, TPropertyType>> property)
        {
            var predicate = PredicateBuilder.New<TEntity>();
            foreach (var item in collection)
            {
                var comparison = Expression.Equal(property.Body, Expression.Constant(item));
                var itemPredicate = Expression.Lambda<Func<TEntity, bool>>(comparison, property.Parameters);
                predicate = predicate.Or(itemPredicate);
            }

            return predicate;
        }
    }
}
