using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace AsyncAwaitDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(FooAsync2().Result);
        }


        async static Task<int> FooAsync()
        {
            return 42;
        }

        static Task<int> FooAsync2()
        {
            var stateMachine = new FooAsyncStateMachine();
            stateMachine.asyncMethodBuilder =  AsyncTaskMethodBuilder<int>.Create();

            stateMachine.asyncMethodBuilder.Start(ref stateMachine);

            return stateMachine.asyncMethodBuilder.Task;
        }

        struct FooAsyncStateMachine : IAsyncStateMachine
        {
            public AsyncTaskMethodBuilder<int> asyncMethodBuilder;
            public void MoveNext()
            {
                asyncMethodBuilder.SetResult(42);
            }

            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {

            }
        }
    }
}
