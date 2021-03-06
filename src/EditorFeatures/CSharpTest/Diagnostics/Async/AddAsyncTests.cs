﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.CodeFixes.Async;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.Diagnostics.Async
{
    public partial class AddAsyncTests : AbstractCSharpDiagnosticProviderBasedUserDiagnosticTest
    {
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void AwaitInVoidMethodWithModifiers()
        {
            var initial =
@"using System;
using System.Threading.Tasks;

class Program 
{
    public static void Test() 
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"using System;
using System.Threading.Tasks;

class Program 
{
    public static async void Test() 
    {
        await Task.Delay(1);
    }
}";
            Test(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void AwaitInTaskMethodNoModifiers()
        {
            var initial =
@"using System;
using System.Threading.Tasks;

class Program 
{
    Task Test() 
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"using System;
using System.Threading.Tasks;

class Program 
{
    async Task Test() 
    {
        await Task.Delay(1);
    }
}";
            Test(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void AwaitInTaskMethodWithModifiers()
        {
            var initial =
@"using System;
using System.Threading.Tasks;

class Program
{
    public/*Comment*/static/*Comment*/Task/*Comment*/Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"using System;
using System.Threading.Tasks;

class Program
{
    public/*Comment*/static/*Comment*/async Task/*Comment*/Test()
    {
        await Task.Delay(1);
    }
}";
            Test(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void AwaitInLambdaFunction()
        {
            var initial =
@"using System;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        Action a = () => Console.WriteLine();
        Func<Task> b = () => [|await Task.Run(a);|]
    }
}";

            var expected =
@"using System;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        Action a = () => Console.WriteLine();
        Func<Task> b = async () => await Task.Run(a);
    }
}";
            Test(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void AwaitInLambdaAction()
        {
            var initial =
@"using System;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        Action a = () => [|await Task.Delay(1);|]
    }
}";

            var expected =
@"using System;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        Action a = async () => await Task.Delay(1);
    }
}";
            Test(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void BadAwaitInNonAsyncMethod()
        {
            var initial =
@"using System.Threading.Tasks;
class Program
{
    void Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"using System.Threading.Tasks;
class Program
{
    async void Test()
    {
        await Task.Delay(1);
    }
}";
            Test(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void BadAwaitInNonAsyncMethod2()
        {
            var initial =
@"using System.Threading.Tasks;
class Program
{
    Task Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"using System.Threading.Tasks;
class Program
{
    async Task Test()
    {
        await Task.Delay(1);
    }
}";
            Test(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void BadAwaitInNonAsyncMethod3()
        {
            var initial =
@"using System.Threading.Tasks;
class Program
{
    Task<int> Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"using System.Threading.Tasks;
class Program
{
    async Task<int> Test()
    {
        await Task.Delay(1);
    }
}";
            Test(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void BadAwaitInNonAsyncMethod4()
        {
            var initial =
@"using System.Threading.Tasks;
class Program
{
    int Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"using System.Threading.Tasks;
class Program
{
    async Task<int> Test()
    {
        await Task.Delay(1);
    }
}";
            Test(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void BadAwaitInNonAsyncMethod5()
        {
            var initial =
@"class Program
{
    void Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"class Program
{
    async void Test()
    {
        await Task.Delay(1);
    }
}";
            Test(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void BadAwaitInNonAsyncMethod6()
        {
            var initial =
@"class Program
{
    Task Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"class Program
{
    async Task Test()
    {
        await Task.Delay(1);
    }
}";
            Test(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void BadAwaitInNonAsyncMethod7()
        {
            var initial =
@"class Program
{
    Task<int> Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"class Program
{
    async Task<int> Test()
    {
        await Task.Delay(1);
    }
}";
            Test(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void BadAwaitInNonAsyncMethod8()
        {
            var initial =
@"class Program
{
    int Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"class Program
{
    async System.Threading.Tasks.Task<int> Test()
    {
        await Task.Delay(1);
    }
}";
            Test(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void BadAwaitInNonAsyncMethod9()
        {
            var initial =
@"class Program
{
    Program Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"class Program
{
    async System.Threading.Tasks.Task<Program> Test()
    {
        await Task.Delay(1);
    }
}";
            Test(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void BadAwaitInNonAsyncMethod10()
        {
            var initial =
@"class Program
{
    asdf Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"class Program
{
    async System.Threading.Tasks.Task<asdf> Test()
    {
        await Task.Delay(1);
    }
}";
            Test(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void AwaitInMember()
        {
            var code =
@"using System.Threading.Tasks;

class Program
{
    var x = [|await Task.Delay(3)|];
}";
            TestMissing(code);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void AddAsyncInDelegate()
        {
            Test(
@"using System ; using System . Threading . Tasks ; class Program { private async void method ( ) { string content = await Task < String > . Run ( delegate ( ) { [|await Task . Delay ( 1000 )|] ; return ""Test"" ; } ) ; } } ",
@"using System ; using System . Threading . Tasks ; class Program { private async void method ( ) { string content = await Task < String > . Run ( async delegate ( ) { await Task . Delay ( 1000 ) ; return ""Test"" ; } ) ; } } ");
        }


        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void AddAsyncInDelegate2()
        {
            Test(
@"using System ; using System . Threading . Tasks ; class Program { private void method ( ) { string content = await Task < String > . Run ( delegate ( ) { [|await Task . Delay ( 1000 )|] ; return ""Test"" ; } ) ; } } ",
@"using System ; using System . Threading . Tasks ; class Program { private void method ( ) { string content = await Task < String > . Run ( async delegate ( ) { await Task . Delay ( 1000 ) ; return ""Test"" ; } ) ; } } ");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public void AddAsyncInDelegate3()
        {
            Test(
@"using System ; using System . Threading . Tasks ; class Program { private void method ( ) { string content = await Task < String > . Run ( delegate ( ) { [|await Task . Delay ( 1000 )|] ; return ""Test"" ; } ) ; } } ",
@"using System ; using System . Threading . Tasks ; class Program { private void method ( ) { string content = await Task < String > . Run ( async delegate ( ) { await Task . Delay ( 1000 ) ; return ""Test"" ; } ) ; } } ");
        }

        internal override Tuple<DiagnosticAnalyzer, CodeFixProvider> CreateDiagnosticProviderAndFixer(Workspace workspace)
        {
            return new Tuple<DiagnosticAnalyzer, CodeFixProvider>(null, new CSharpAddAsyncCodeFixProvider());
        }
    }
}
