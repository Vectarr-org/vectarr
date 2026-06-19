using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess.IInfrastructure;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.DataAccess.IRepositories
{
    public interface IQuoteRevisionRepository : IEntityRepository<QuoteRevision, QuoteRevisionModel>
    {
    }
}
