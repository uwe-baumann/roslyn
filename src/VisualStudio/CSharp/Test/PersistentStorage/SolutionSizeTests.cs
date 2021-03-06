﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.LanguageServices.Implementation.SolutionSize;
using Xunit;

namespace Microsoft.CodeAnalysis.UnitTests.WorkspaceServices
{
    public class SolutionSizeTests
    {
        [Fact]
        public void Test_SolutionSize()
        {
            var expected = 12345;
            var solution = CreateSolution(expected);

            var analyzer = new SolutionSizeTracker.IncrementalAnalyzer();

            // initialize
            analyzer.NewSolutionSnapshotAsync(solution, CancellationToken.None).Wait();
            AddSolution(analyzer, solution);

            var size = analyzer.GetSolutionSize(solution.Id);
            Assert.Equal(expected, size);
        }

        [Fact]
        public void Test_SolutionSize_Update()
        {
            var expected = 12345;
            var solution = CreateSolution(expected);

            var analyzer = new SolutionSizeTracker.IncrementalAnalyzer();

            // initialize
            analyzer.NewSolutionSnapshotAsync(solution, CancellationToken.None).Wait();
            AddSolution(analyzer, solution);

            // update document
            var document = solution.Projects.First().Documents.First();
            var length = document.GetSyntaxTreeAsync().Result.Length;

            var text = SourceText.From(new string('2', 1000));
            var newDocument = document.WithText(text);

            analyzer.AnalyzeSyntaxAsync(newDocument, CancellationToken.None).Wait();

            var size = analyzer.GetSolutionSize(solution.Id);
            Assert.Equal(expected - length + text.Length, size);
        }

        [Fact]
        public void Test_RemoveDocument()
        {
            var expected = 12345;
            var solution = CreateSolution(expected);

            var analyzer = new SolutionSizeTracker.IncrementalAnalyzer();

            // initialize
            analyzer.NewSolutionSnapshotAsync(solution, CancellationToken.None).Wait();
            AddSolution(analyzer, solution);

            // remove document
            var document = solution.Projects.First().Documents.First();
            analyzer.RemoveDocument(document.Id);

            var size = analyzer.GetSolutionSize(solution.Id);

            var length = document.GetSyntaxTreeAsync().Result.Length;
            Assert.Equal(expected - length, size);
        }

        private static void AddSolution(SolutionSizeTracker.IncrementalAnalyzer analyzer, Solution solution)
        {
            foreach (var document in solution.Projects.SelectMany(p => p.Documents))
            {
                analyzer.AnalyzeSyntaxAsync(document, CancellationToken.None).Wait();
            }
        }

        private static Solution CreateSolution(int solutionSize)
        {
            var info = SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create());

            var workspace = new AdhocWorkspace();
            workspace.AddSolution(info);

            var solution = workspace.CurrentSolution;
            var project = solution.AddProject("proj1", "proj1", LanguageNames.CSharp);

            var current = 0;
            for (var i = 0; true; i++)
            {
                var size = current + 1234;
                if (current + size >= solutionSize)
                {
                    break;
                }

                project = project.AddDocument("doc" + i, new string('a', size)).Project;
                current += size;
            }

            var left = solutionSize - current;
            if (left > 0)
            {
                project = project.AddDocument("docLast", new string('a', left)).Project;
            }

            return project.Solution;
        }
    }
}
