using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using YasudaAnalysis;

namespace YasudaApi.Controllers
{
    public class ValuesController : ApiController
    {
        // POST api/values 
        public async Task<int> Post([FromBody]string value)
        {
            //string partitionKey = Guid.NewGuid().ToString();
            Random ram = new Random();
            int partitionKey = ram.Next();
            IYasudaAnalysis analysis = ServiceProxy.Create<IYasudaAnalysis>(
                new Uri("fabric:/Services/YasudaAnalysis"),
                new ServicePartitionKey(partitionKey));
            await analysis.StartAnalysis(partitionKey);

            return partitionKey;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

    }
}
