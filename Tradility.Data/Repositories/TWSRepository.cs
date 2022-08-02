using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradility.Data.Models;
using Tradility.TWS;

namespace Tradility.Data.Repositories
{
    public class TWSRepository<T> : BaseRepository<T> where T : BaseModel
    {
        public Client TWSClient { get; }

        public TWSRepository(Client twsClient)
        {
            TWSClient = twsClient;
        }
    }
}
