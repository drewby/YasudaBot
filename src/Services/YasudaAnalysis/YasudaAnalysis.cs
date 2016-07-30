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

        public YasudaAnalysis(StatefulServiceContext context)
            : base(context)
        { }

        public Task<int> GetComedy(int taskId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetStatus(int taskId)
        {
            throw new NotImplementedException();
        }

        public async Task StartAnalysis(int partitionKey)
        {
            //this.analysisCollection =
            //    await this.StateManager.GetOrAddAsync<IReliableDictionary<string, AnalysisState>>("statuscollection");

            using (ITransaction tx = this.StateManager.CreateTransaction())
            {
                await analysisCollection.AddAsync(tx, partitionKey, new AnalysisState());
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

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    //var result = await myDictionary.TryGetValueAsync(tx, "Counter");
                    ///////書きかけ
                    var enumAnalysis = await this.analysisCollection.CreateEnumerableAsync(tx);
                    foreachasync

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
        Task StartAnalysis(int partitionKey /*, byte[] aaa*/);

        Task<int> GetStatus(int partitionKey);

        Task<int> GetComedy(int partitionKey);
    }

    public class AnalysisState
    {
        public AnalysisState()
        {
            status = 0;
        }

        public int status { get; set; } // 0 started, 1 got-face-information, 2 got-comedy
    }
}
