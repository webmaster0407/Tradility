using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tradility.Common.Extensions;
using Tradility.Data.Models;
using Tradility.Data.Repositories.Abstractions;
using Tradility.Data.Services;
using Tradility.TWS;
using Tradility.TWS.API;
using Tradility.TWS.Requests;

namespace Tradility.Data.Repositories
{
    public class PositionRepository : BaseRepository<PositionModel>, ILoadableRepository
    {
        private readonly Client client; // TODO TWSRepo Base
        private readonly DumpService dump; // TODO Delete

        public PositionRepository(Client client, DumpService dumpService)
        {
            this.client = client;
            this.dump = dumpService;
        }

        public async Task LoadAsync()
        {
            //var positionsDebug = await GetPositionFromJSONAsync();
            //Items = new(positionsDebug);
            //return;

            await client.ConnectAsync();

            var request = new PositionsRequest(client);
            var positionsList = await request.GetAsync();
            await dump.SaveAsync("Positions", positionsList.Select(x => new
            {
                x.Account,
                x.AvgCost,
                x.Pos,
                x.Contract
            }).ToList());
            var positions = positionsList.Select(x => new PositionModel
            {
                Account = x.Account,
                AvgCost = x.AvgCost,
                Pos = x.Pos,
                Contract = x.Contract
            });

            Items = new(positions);
        }

        private async Task<List<PositionModel>> GetPositionFromJSONAsync()
        {
            var file = await File.ReadAllTextAsync("positions.json");
            var positionsList = file.ToModel<List<PositionModel>>();

            return positionsList;
        }
    }
}
