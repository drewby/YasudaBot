using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using YasudaAnalysis;

namespace YasudaApi.Controllers
{
    public class AnalysisController : ApiController
    {
        // POST api/analysis
        public async Task<HttpResponseMessage> Post()
        {
            //string partitionKey = Guid.NewGuid().ToString();
            Random ram = new Random();
            int partitionKey = ram.Next();
            byte[] image = await Request.Content.ReadAsByteArrayAsync();
            IYasudaAnalysis analysis = ServiceProxy.Create<IYasudaAnalysis>(
                new Uri("fabric:/Services/YasudaAnalysis"),
                new ServicePartitionKey(partitionKey));
            await analysis.StartAnalysis(partitionKey, image);

            var bodystr = @"{""TaskId"":""" + partitionKey + @"""}";
            var bodycont = new StringContent(bodystr);
            bodycont.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var resp = new HttpResponseMessage();
            resp.Content = bodycont;
            //resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return resp;
        }

        // GET api/values/5
        public async Task<AnalysisState> Get(int id)
        {
            IYasudaAnalysis analysis = ServiceProxy.Create<IYasudaAnalysis>(
                new Uri("fabric:/Services/YasudaAnalysis"),
                new ServicePartitionKey(id));
            var res = await analysis.GetResult(id);
            return res;
        }

    }
}
