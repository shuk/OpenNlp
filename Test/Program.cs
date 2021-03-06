﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenNLP.Tools.Chunker;
using OpenNLP.Tools.Parser;
using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.SentenceDetect;
using OpenNLP.Tools.Tokenize;
using OpenNLP.Tools.Trees;
using SharpEntropy;
using SharpEntropy.IO;

namespace Test
{
    internal class Program
    {
        private static readonly string currentDirectory = Environment.CurrentDirectory + "/../../";

        private static void Main(string[] args)
        {
            /*FileStream ostrm;
            StreamWriter writer;
            TextWriter oldOut = Console.Out;
            try
            {
                ostrm = new FileStream("C:\\Users\\Alexandre\\Desktop\\vs_output_2.txt", FileMode.OpenOrCreate, FileAccess.Write);
                writer = new StreamWriter(ostrm);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open Redirect.txt for writing");
                Console.WriteLine(e.Message);
                return;
            }
            Console.SetOut(writer);*/


            /*// read file
            var tokenizerTrainingFilePath = currentDirectory + "Input/tokenizer.train";
            var outputFilePath = currentDirectory + "Output/EnglishTok.nbin";
            MaximumEntropyTokenizer.Train(tokenizerTrainingFilePath, outputFilePath);*/

            // test detokenization
            /*var tokens = new List<string>() {"do", "n't", "commit"};
            var detokenizer = new DictionaryDetokenizer();
            var result = detokenizer.Detokenize(tokens.ToArray());
            Console.WriteLine(result);*/

            /*// train model file
            var inputFilePath = currentDirectory + "Input/sentences.train";
            var outputFilePath = currentDirectory + "Output/" + Path.GetFileNameWithoutExtension(inputFilePath) + ".nbin";
            var iterations = 100;
            var cut = 5;
            var endOfSentenceScanner = new CharactersSpecificEndOfSentenceScanner();
            Console.WriteLine("Training model...");
            var model = MaximumEntropySentenceDetector.TrainModel(inputFilePath, iterations, cut, endOfSentenceScanner);
            Console.WriteLine("Writing output file '{0}'...", outputFilePath);
            new BinaryGisModelWriter().Persist(model, outputFilePath);
            Console.WriteLine("Output file written.");*/

            /*// tokenize tests
            var modelPath = currentDirectory + "../Resources/Models/";
            var tokenizer = new EnglishMaximumEntropyTokenizer(modelPath + "EnglishTok.nbin");

            var input = "It was built of a bright brick throughout; its skyline was fantastic, and even its ground plan was wild.";
            var tokens = tokenizer.Tokenize(input);
            Console.WriteLine(string.Join(" | ", tokens));*/


            // detect tokenization issues
            /*var pathToFile = currentDirectory + "Input/tokenizerIssues.txt";
            var modelPath = currentDirectory + "../Resources/Models/";
            var tokenizer = new EnglishMaximumEntropyTokenizer(modelPath + "EnglishTok.nbin");
            var allLines = File.ReadAllLines(pathToFile);
            foreach (var line in allLines)
            {
                var tokens = tokenizer.Tokenize(line);
                Console.WriteLine(string.Join(" | ", tokens));
            }*/

            // parsing
            /*var sentence = "This is a generic bank response, which indicates simply that they are not willing to accept the transaction.";
            var tokenizer = new EnglishMaximumEntropyTokenizer(currentDirectory + "../Resources/Models/EnglishTok.nbin");
            var tokens = tokenizer.Tokenize(sentence);
            var modelPath = currentDirectory + "../Resources/Models/";
            var parser = new OpenNLP.Tools.Parser.EnglishTreebankParser(modelPath, true, false);
            var parse = parser.DoParse(tokens);
            // Extract dependencies from lexical tree
            var tlp = new PennTreebankLanguagePack();
            var gsf = tlp.GrammaticalStructureFactory();
            var tree = new ParseTree(parse);
            Console.WriteLine(tree);
            var gs = gsf.NewGrammaticalStructure(tree);
            var dependencies = gs.TypedDependencies();

            foreach (var dep in dependencies)
            {
                Console.WriteLine(dep);
            }*/

            // multi threaded sentence detection
            var sentences = new List<string>()
            {
                "No. Red.",
                "A former Redbone. Price tag's gonna be steep.",
                "I mean, she's got eyes.",
                "- You're kicking me out of my own house? - It's your aunt Ginny's house.",
                "Alchemillas. Or something that smells nice.",
                "It's all right.",
                "It's me, Yara.",
                "You can't trick me.",
                "And do you suspect Byamba, son of the Khan, to have blood on his hands?",
                "We traveled side by side.",
                "Is that accurate, Byamba?",
                "Were you always at the Latin's side on your journey?",
                "I was asked to be here and I came.",
                "But you cannot compel me to be party to this.",
                "(Grunts sharply)",
                "No, we were not side by side."
            };
            var sentenceDetector = new EnglishMaximumEntropySentenceDetector(currentDirectory + "../Resources/Models/EnglishSD.nbin");
            var modelPath = currentDirectory + "../Resources/Models/";
            //var parser = new EnglishTreebankParser(modelPath, true, false);

            Parallel.ForEach(sentences, sentence =>
            {
                var positions = sentenceDetector.SentencePositionDetect(sentence);
                //var parse = parser.DoParse(positions);

                Console.WriteLine("'{0}'", sentence);
                Console.WriteLine("{0}", string.Join(" | ", positions));
                //Console.WriteLine("{0}", parse);
                Console.WriteLine("--");
            });

            Console.WriteLine("===========");
            Console.WriteLine("OK");
            Console.ReadKey();
        }

        /*private void TestDechunk()
        {
            // detokenize
            var inputs = new string[]
            {
                "- Harry's your sister. - Look, what exactly am I supposed to be doing here?",
                "\"Piss off!\"",
                "- Sorry Mrs. Hudson, I'll skip the tea. Off out. - Both of you?",
                "I love playing half-life; that's just who I am!",
                "That's why I... have just begun to write a book.",
                "And they lived happily ever after...",
                "It's gonna be $1.5 sir."
            };

            // 
            var tokenizer = new EnglishMaximumEntropyTokenizer(currentDirectory + "../Resources/Models/EnglishTok.nbin");
            var chunker = new EnglishTreebankChunker(currentDirectory + "../Resources/Models/EnglishChunk.nbin");
            var dechunker = new RegexDictionaryDechunker();
            var detokienizer = new DictionaryDetokenizer();
            var englishPosPath = currentDirectory + "../Resources/Models/EnglishPOS.nbin";
            var tagDictPath = currentDirectory + "../Resources/Models/Parser/tagdict";
            var posTagger = new EnglishMaximumEntropyPosTagger(englishPosPath, tagDictPath);

            foreach (var input in inputs)
            {
                string[] tokens = tokenizer.Tokenize(input);
                string[] tags = posTagger.Tag(tokens);

                var chunks = chunker.GetChunks(tokens, tags);
                var chunksStrings = chunks
                    .Select(ch => detokienizer.Detokenize(ch.TaggedWords.Select(tw => tw.Word).ToArray()))
                    .ToArray();
                var output = dechunker.Dechunk(chunksStrings);
                Console.WriteLine("input: " + input);
                Console.WriteLine("chunks: " + string.Join(" | ", chunks));
                Console.WriteLine("ouput: " + output);
                Console.WriteLine("--");
            }
        }*/
    }
}