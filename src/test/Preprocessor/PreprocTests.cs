using System.ComponentModel;
using System.Reflection;
using Pdcl.Core;
using Pdcl.Core.Preproc;
using Pdcl.Core.Text;

namespace Pdcl.Test;

public partial class PreprocTest
{
    [Fact]
    public void HandleMacro_Test()
    {
        using SourceStream stream = new SourceStream(GetRelativePath() + "Preprocessor/macro.pdcl");
        Preprocessor preproc = new Preprocessor(stream);

        // Expected names
        string[] macrosNames = { "PI", "struct0_", "someArgedMacro", "NonFirstT" };


        for (int i = 0; i < macrosNames.Length; i++)
        {
            while (stream.Peek() != Preprocessor.DirectiveLiteral) 
                if (!stream.handleLeadingTrivia()) stream.Position++;

            IAnalyzerResult<IDirective, Preprocessor.PreprocStatusCode> macro = preproc.TryParseDirective().Result;

            Assert.True(!macro.IsFailed && macrosNames[i] == macro.Value!.Name);
        }
        // Non-first token
        /* Assert.True(macro.Status == Preprocessor.PreprocStatusCode.NonFirstToken); */
    }
    [Fact]
    public void HandleIfdef_Test()
    {
        // HandleMacro_Test is a dependency and has to work fine
        using SourceStream stream = new SourceStream(GetRelativePath() + "Preprocessor/ifdef.pdcl");

        Preprocessor preproc = new Preprocessor(stream);
        preproc.DefinedMacros["someArgedMacro"] = new Macro("someArgedMacro", default!, default);

        IAnalyzerResult<IDirective, Preprocessor.PreprocStatusCode> dir = preproc.TryParseDirective().Result;
        Assert.True(!dir.IsFailed && ((Ifdef)dir.Value!).Result == false);
        /* 
            #ifdef ignored
            #endif 
        */
        while (stream.Peek() != Preprocessor.DirectiveLiteral)
            if(!stream.handleLeadingTrivia()) stream.Position++;
        dir = preproc.TryParseDirective().Result;
        Assert.True(!dir.IsFailed && ((Ifdef)dir.Value!).Result);
        /*
            #ifdef someArgedMacro
            #endif
        */
        while (stream.Peek() != Preprocessor.DirectiveLiteral)
            if(!stream.handleLeadingTrivia()) stream.Position++;

        Assert.Throws<AggregateException>(() => preproc.TryParseDirective().Result);
        /*
            #ifndef someArgedMacro
                #ifdef someArgedMacro
                #endif
        */
    }
}