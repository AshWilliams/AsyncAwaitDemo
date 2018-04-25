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
            Console.WriteLine(BarAsyncCompiled().Result);
            Console.WriteLine(FooAsyncCompiled().Result);
        }

        async static Task<int> BarAsync()
        {
            return 42;
        }

        static Task<int> BarAsyncCompiled()
        {
            var stateMachine = new BarAsyncStateMachine();
            stateMachine.asyncMethodBuilder = AsyncTaskMethodBuilder<int>.Create();
            stateMachine.asyncMethodBuilder.Start(ref stateMachine);

            return stateMachine.asyncMethodBuilder.Task;
        }

        struct BarAsyncStateMachine : IAsyncStateMachine
        {
            public AsyncTaskMethodBuilder<int> asyncMethodBuilder;

            public void MoveNext()
            {
                asyncMethodBuilder.SetResult(42); 
            }

            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                asyncMethodBuilder.SetStateMachine(stateMachine);
            }
        }

        async static Task<int> FooAsync()
        {
            await Task.Delay(2000);
            return 42;
        }

        static Task<int> FooAsyncCompiled()
        {
            var stateMachine = new FooAsyncStateMachine();
            stateMachine.asyncMethodBuilder =  AsyncTaskMethodBuilder<int>.Create();

            stateMachine.asyncMethodBuilder.Start(ref stateMachine);

            return stateMachine.asyncMethodBuilder.Task;
        }

        struct FooAsyncStateMachine : IAsyncStateMachine
        {
            public AsyncTaskMethodBuilder<int> asyncMethodBuilder;

            private int state;
            private TaskAwaiter awaiter;
            public void MoveNext()
            {
                if(state == 0)
                {
                    awaiter = Task.Delay(2000).GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        state = 1;
                        goto state1;
                    }
                    else
                    {
                        state = 1;
                        asyncMethodBuilder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
                    }
                    return;
                }
state1:
                if(state == 1)
                {
                    awaiter.GetResult();
                    asyncMethodBuilder.SetResult(42);
                    return;
                }
                
            }

            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                asyncMethodBuilder.SetStateMachine(stateMachine);
            }
        }
    }
}
