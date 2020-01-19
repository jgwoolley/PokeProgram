using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CsvToJson;
using FileHelpers;

namespace PokeProgram
{
    public class Runner
    {

        public static void Main(string[] args) //FileWrapper DirectoryWrapper
        {
            DirectoryWrapper jsonDir = DirectoryWrapper.Create().GetParent(4);
            jsonDir.Add("data") ;
            DirectoryWrapper csvDir = jsonDir.GetChildDirectory("csv");
            DirectoryWrapper customCsvDir = jsonDir.GetChildDirectory("custom_csv");
            jsonDir.Add("json");

            FileHelper.WriteLine(jsonDir);
            FileHelper.WriteLine(csvDir);
            FileHelper.WriteLine(customCsvDir);

            ProcessCSVs(csvDir, customCsvDir, jsonDir);
        }

        public static void ProcessCSVs(DirectoryWrapper csvDir, DirectoryWrapper customCsvDir, DirectoryWrapper jsonDir)
        {
            ProccessRegularCSVs(csvDir, jsonDir);

            csvDir.GetChildFile("pokemon_moves.csv");

            FileWrapper pokemonMovesFile = csvDir.GetChildFile("pokemon_moves.csv");

        }

        public static void ProccessRegularCSVs(DirectoryWrapper csvDir, DirectoryWrapper jsonDir)
        {
            Dictionary<string, JObject> processedCsv = CsvToJsonHelper.ProcessCSVDirectory(csvDir, jsonDir);
            foreach (KeyValuePair<string, JObject> keyValue in processedCsv)
            {
                string tableName = keyValue.Key;
                JObject tableJson = keyValue.Value;

                FileWrapper jsonFile = csvDir.GetChildFile(tableName + ".json");

                CsvToJsonHelper.WriteJson(jsonFile.FullName, tableJson);
            }
        }
    }
}