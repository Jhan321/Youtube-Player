using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Shouldly;
using Unity.Coding.Cli;
using Unity.Coding.Tests;
using Unity.Coding.Utils;

namespace CliApp
{
    class CliTests : TestFileSystemFixture
    {
        static StringLogger Execute(params string[] args)
        {
            var packageDir = TestContext
                .CurrentContext.TestDirectory.ToNPath()
                .ParentContaining(@"Packages\com.unity.coding", true)
                .DirectoryMustExist()
                .ToString();

            var appArgs = args.ToList();
            appArgs.Add("--package-root");
            appArgs.Add(packageDir);

            var logger = new StringLogger();
            App.Execute(appArgs, logger);
            return logger;
        }

        [Test]
        public void Format_WithoutFilesSpecified_ReturnsError() =>
            Execute("format")
                .ErrorsAsString
                .ShouldContain("Usage:");

        [Test]
        public void Format_WithNonexistentFile_ReturnsError() =>
            Execute("format", "does_not_exist.cs")
                .ErrorsAsString
                .ShouldContain("File doesn't exist");

        [Test]
        public void Format_WithFolder_ReturnsError() =>
            Execute("format", ".")
                .ErrorsAsString
                .ShouldContain("Path is a directory");

        const string k_UnformattedSource = "\t    class C {\n  };\n";
        const string k_FormattedSource = "class C\n{\n};\n";

        [Test]
        public void Format_WithoutPackageRootSpecified_DoesNotThrow()
        {
            var logger = new StringLogger();
            App.Execute(new[] { "format", BaseDir.Combine("file").WriteAllText("") }, logger);

            logger.ErrorsAsString.ShouldBeEmpty();
            logger.InfosAsString.ShouldBeEmpty();
        }

        [Test]
        public void Format_WithNoEditorConfig_DoesNothing()
        {
            var path = BaseDir.Combine("file.txt").WriteAllText(k_UnformattedSource);

            var result = Execute("format", path);

            result.ErrorsAsString.ShouldBeEmpty();
            result.InfosAsString.ShouldBeEmpty();
            path.ReadAllText().ShouldBe(k_UnformattedSource);
        }

        [Test]
        public void Format_WithBadPackagePath_ReturnsError()
        {
            // minimum required to trigger need for coding package guts
            WriteRootEditorConfig("[*]", "formatters=uncrustify");

            var logger = new StringLogger();
            App.Execute(new[]
            {
                "format", "--package-root", "does_not_exist",
                BaseDir.Combine("file").WriteAllText("")
            }, logger);

            logger.ErrorsAsString.ShouldContain("Invalid package root");
        }

        [Test]
        public void Format_WithEditorConfig_FormatsFile()
        {
            WriteRootEditorConfig("[*]", "formatters=uncrustify,generic");
            var path = BaseDir.Combine("file.cs").WriteAllText(k_UnformattedSource);

            var result = Execute("format", path);

            result.ErrorsAsString.ShouldBeEmpty();
            result.InfosAsString.ShouldBeEmpty();
            path.ReadAllText().ShouldBe(k_FormattedSource);
        }

        [Test]
        public void Format_WithMultipleFilesIncludingOneBadPath_FormatsValidAndErrorsForBad()
        {
            WriteRootEditorConfig("[*]", "formatters=uncrustify,generic");

            var paths = new[]
            {
                BaseDir.Combine("file.cs").WriteAllText(k_UnformattedSource),
                BaseDir.Combine("file2.cs"),
                BaseDir.Combine("file3.cs").WriteAllText(k_UnformattedSource),
            };

            var result = Execute("format", paths[0], paths[1], paths[2]);

            result.Errors.Count.ShouldBe(1);
            result.ErrorsAsString.ShouldMatch(@"File doesn't exist: .*file2\.cs");
            result.InfosAsString.ShouldBeEmpty();

            paths[0].ReadAllText().ShouldBe(k_FormattedSource);
            paths[2].ReadAllText().ShouldBe(k_FormattedSource);
        }

        [Test]
        public void Format_WithBatchFileInputWithDifferentEOL_FormatsExistingFiles()
        {
            var file1 = BaseDir.Combine("file.cs");
            var file2 = BaseDir.Combine("file2.cs");
            var file3 = BaseDir.Combine("file3.cs");
            file1.WriteAllText(k_UnformattedSource);
            file3.WriteAllText(k_UnformattedSource);

            var paths = new[]
            {
                file1,
                file2,
                file3
            };

            var batchFileName = BaseDir.Combine("batchfile.txt");
            var content = $"{paths[0]}\n{paths[1]}\r\n{paths[2]}\n";
            File.WriteAllText(batchFileName, content);

            var logger = Execute("format", "--batch-file", batchFileName);

            logger.Errors.Count.ShouldBe(1);
            logger.ErrorsAsString.ShouldMatch(@"File doesn't exist: .*file2\.cs");
            logger.InfosAsString.ShouldBeEmpty();

            paths[0].ReadAllText().ShouldBe(k_FormattedSource);
            paths[2].ReadAllText().ShouldBe(k_FormattedSource);
        }

        [Test]
        public void Format_WithBatchFileInputIncorrectFilePath_LogsError()
        {
            var batchFileName = BaseDir.Combine("non-existing-file.txt");

            var logger = Execute("format", "--batch-file", batchFileName);

            logger.Errors.Count.ShouldBe(1);
            logger.ErrorsAsString.ShouldContain($"File does not exist: {batchFileName}");
            logger.InfosAsString.ShouldBeEmpty();
        }

        [Test]
        public void Format_Validate_FailsOnUnformattedFile()
        {
            var file1 = BaseDir.Combine("file.cs");
            var file2 = BaseDir.Combine("file2.cs");
            file1.WriteAllText(k_UnformattedSource);
            file2.WriteAllText(k_FormattedSource);

            var logger = Execute("format", "--validate", file1, file2);

            logger.Errors.Count.ShouldBe(1);
            logger.ErrorsAsString.ShouldContain("Validation failed");
            logger.InfosAsString.ShouldBeEmpty();

            file1.ReadAllText().ShouldBe(k_UnformattedSource);
            file2.ReadAllText().ShouldBe(k_FormattedSource);
        }

        [Test]
        public void Format_Validate_FailsOnLogMessage()
        {
            var file1 = BaseDir.Combine("file.cs");
            var file2 = BaseDir.Combine("file2.cs");
            file1.WriteAllText(k_FormattedSource);

            var logger = Execute("format", "--validate", file1, file2);

            logger.Errors.Count.ShouldBe(1);
            logger.ErrorsAsString.ShouldContain(" doesn't exist");
            logger.InfosAsString.ShouldBeEmpty();
        }

        [Test]
        public void Format_Validate_SucceedsOnFormattedFiles()
        {
            var file1 = BaseDir.Combine("file.cs");
            file1.WriteAllText(k_FormattedSource);

            var logger = Execute("format", "--validate", file1);

            logger.Errors.Count.ShouldBe(0);
            logger.InfosAsString.ShouldBeEmpty();
            file1.ReadAllText().ShouldBe(k_FormattedSource);
        }
    }
}
