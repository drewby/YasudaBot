using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ProjectOxford.Face;
using System.IO;
//using Microsoft.ServiceFabric.Services.Remoting.Client;
//using Microsoft.ServiceFabric.Services.Client;

namespace YasudaAnalysis
{
    /// <summary>
    /// Service Fabric ランタイムによって、このクラスのインスタンスがサービス レプリカごとに作成されます。
    /// </summary>
    internal sealed class YasudaAnalysis : StatefulService, IYasudaAnalysis
    {
        private IReliableDictionary<int, AnalysisState> analysisCollection = null;
        private IReliableDictionary<int, byte[]> byteCollection = null;

        public YasudaAnalysis(StatefulServiceContext context)
            : base(context)
        { }

        public async Task<AnalysisState> GetResult(int partitionKey)
        {
            AnalysisState result;
            using (ITransaction tx = this.StateManager.CreateTransaction())
            {
                var state = await analysisCollection.TryGetValueAsync(tx, partitionKey);
                result = state.Value;
                await tx.CommitAsync();
            }
            return result;
        }

        public async Task StartAnalysis(int partitionKey, byte[] imagebyte)
        {
            //this.analysisCollection =
            //    await this.StateManager.GetOrAddAsync<IReliableDictionary<string, AnalysisState>>("statuscollection");

            using (ITransaction tx = this.StateManager.CreateTransaction())
            {
                var state = new AnalysisState();
                //var state = new AnalysisState(imagebyte);
                await analysisCollection.AddAsync(tx, partitionKey, state);
                await byteCollection.AddAsync(tx, partitionKey, imagebyte);
                await tx.CommitAsync();
            }
        }

        /// <summary>
        ///オプションで、このサービスのレプリカに対するリスナー (HTTP、Service Remoting、WCF など) の作成を無視して、クライアント要求やユーザー要求を処理します。
        /// </summary>
        /// <remarks>
        ///サービスの通信の詳細については、http://aka.ms/servicefabricservicecommunication を参照してください
        /// </remarks>
        /// <returns>リスナーのコレクション。</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            //return new ServiceReplicaListener[0];
            return new[]
            {
                new ServiceReplicaListener(
                    context => this.CreateServiceRemotingListener<YasudaAnalysis>(context),
                    "Remoting1"
                )
            };

        }

        /// <summary>
        /// これは、サービス レプリカのメイン エントリ ポイントです。
        /// このメソッドは、サービスのこのレプリカがプライマリになって、書き込み状態になると実行されます。
        /// </summary>
        /// <param name="cancellationToken">Service Fabric がこのサービス レプリカをシャットダウンする必要が生じると、キャンセルされます。</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: 次のサンプル コードを独自のロジックに置き換えるか、
            //       サービスで不要な場合にはこの RunAsync オーバーライドを削除します。

            //var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");
            this.analysisCollection =
                await this.StateManager.GetOrAddAsync<IReliableDictionary<int, AnalysisState>>("statuscollection");
            this.byteCollection =
                await this.StateManager.GetOrAddAsync<IReliableDictionary<int, byte[]>>("bytecollection");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    //var result = await myDictionary.TryGetValueAsync(tx, "Counter");
                    var enumAnalysis = await this.analysisCollection.CreateEnumerableAsync(tx);
                    var aeor = enumAnalysis.GetAsyncEnumerator();
                    while(await aeor.MoveNextAsync(cancellationToken))
                    {
                        var item = aeor.Current;
                        if(item.Value.status == 0)
                        {
                            // Face API
                            IFaceServiceClient fclient = new FaceServiceClient("3fc374c191c042f78caf27cf8b7e8900");
                            byte[] image = await this.byteCollection.GetOrAddAsync(tx, item.Key, new byte[0]);
                            using (Stream memstream = new MemoryStream(image))
                            {
                                var requiredFaceAttributes = new FaceAttributeType[] {
                                    FaceAttributeType.Age,
                                    FaceAttributeType.Gender,
                                    FaceAttributeType.Smile
                                };
                                var res = await fclient.DetectAsync(memstream, returnFaceAttributes: requiredFaceAttributes);
                                item.Value.age = res[0].FaceAttributes.Age;
                                item.Value.gender = res[0].FaceAttributes.Gender;
                                item.Value.smile = res[0].FaceAttributes.Smile;
                            }
                            item.Value.status = 1;
                        }
                    }

                    //ServiceEventSource.Current.ServiceMessage(this, "Current Counter Value: {0}",
                    //    result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    //await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // CommitAsync の呼び出し前に例外がスローされると、トランザクションが強制終了し、すべての変更が 
                    // 破棄されます。セカンダリ レプリカには何も保存されません。
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }

    public interface IYasudaAnalysis : IService
    {
        Task StartAnalysis(int partitionKey, byte[] imagebyte);
        Task<AnalysisState> GetResult(int partitionKey);
    }

    public class AnalysisState
    {
        public AnalysisState()
        {
            status = 0;
        }

        public int status { get; set; } // 0 started, 1 got-face-information
        public double age { get; set; }
        public string gender { get; set; }
        public double smile { get; set; }
    }
}
