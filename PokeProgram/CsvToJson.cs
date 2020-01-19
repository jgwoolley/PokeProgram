using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using FileHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CsvToJson
{
    public class CsvToJsonHelper
    {

        private static void SwapCsv(string swapCsvDir, string toCsvDir, string fileName, int fromIndex, int toIndex)
        {
            string swapCsvFile = swapCsvDir + Path.DirectorySeparatorChar + fileName + ".csv";
            string csvFile = toCsvDir + Path.DirectorySeparatorChar + fileName + ".csv";

            Console.WriteLine(Directory.Exists(csvFile) + " " + toCsvDir);

            using System.IO.StreamWriter fileWriter = new System.IO.StreamWriter(csvFile);
            using StreamReader streamReader = new StreamReader(new FileStream(swapCsvFile, FileMode.Open, FileAccess.Read), Encoding.UTF8);
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                string[] row = line.Split(",");
                string fromText = row[fromIndex];
                string toText = row[toIndex];
                row[toIndex] = fromText;
                row[fromIndex] = toText;

                StringBuilder buff = new StringBuilder();
                buff.Append(row[0]);
                for (int index = 1; index < row.Length; index++)
                {
                    buff.Append("," + row[index]);
                }
                fileWriter.WriteLine(buff.ToString());
            }
        }

        public static Dictionary<string, JObject> ProcessCSVDirectory(DirectoryWrapper csvDirWrapper, DirectoryWrapper jsonDirWrapper)
        {
            DirectoryInfo csvDir = csvDirWrapper.CreateInfo();
            DirectoryInfo jsonDir = jsonDirWrapper.CreateInfo();

            if (!csvDir.Exists)
            {
                throw new ArgumentException("Directory does not exist: " + csvDir + ".");
            }

            if (!jsonDir.Exists)
            {
                throw new ArgumentException("Directory does not exist: " + jsonDir + ".");
            }

            Dictionary<string, JObject> processedCsv = new Dictionary<string, JObject>();

            foreach (FileInfo csvFile in csvDir.GetFiles())
            {
                FileWrapper csvWrapper = new FileWrapper(csvFile);
                JObject tableCsv = ProcessCSV(csvWrapper);

                processedCsv.Add(csvWrapper.FullName, tableCsv);
            }

            return processedCsv;
        }

        public static JObject FindFields(Dictionary<string, HashSet<string>> fields)
        {
            JObject specJson = new JObject();
            foreach (KeyValuePair<string, HashSet<string>> pair in fields)
            {
                string fieldName = pair.Key;
                JObject fieldJson = new JObject();
                JArray exampleArray = null;
                HashSet<string> allFieldValues = pair.Value;
                fieldJson["count"] = allFieldValues.Count;

                bool foundType = false;

                if (allFieldValues.Count == 2 && allFieldValues.Contains("0") && allFieldValues.Contains("1"))
                {
                    exampleArray = new JArray();
                    foundType = true;
                    fieldJson["type"] = "int bool";
                    foreach (string fieldValue in allFieldValues)
                    {
                        if (exampleArray.Count <= 10)
                        {
                            exampleArray.Add(fieldValue);
                        }
                    }
                }

                if (!foundType)
                {
                    exampleArray = new JArray();
                    foundType = true;
                    foreach (string fieldValueString in allFieldValues)
                    {
                        if (bool.TryParse(fieldValueString, out bool fieldValue))
                        {
                            if (exampleArray.Count <= 10)
                            {
                                exampleArray.Add(fieldValue);
                            }
                        }
                        else
                        {
                            foundType = false;
                            break;
                        }
                    }
                    if (foundType)
                    {
                        fieldJson["type"] = "bool";
                    }
                }

                if (!foundType)
                {
                    exampleArray = new JArray();
                    foundType = true;
                    foreach (string fieldValueString in allFieldValues)
                    {
                        if (int.TryParse(fieldValueString, out int fieldValue))
                        {
                            if (exampleArray.Count <= 10)
                            {
                                exampleArray.Add(fieldValue);
                            }
                        }
                        else
                        {
                            foundType = false;
                            break;
                        }
                    }
                    if (foundType)
                    {
                        fieldJson["type"] = "int";
                    }
                }

                if (!foundType)
                {
                    exampleArray = new JArray();
                    foundType = true;
                    foreach (string fieldValueString in allFieldValues)
                    {
                        if (double.TryParse(fieldValueString, out double fieldValue))
                        {
                            if (exampleArray.Count <= 10)
                            {
                                exampleArray.Add(fieldValue);
                            }
                        }
                        else
                        {
                            foundType = false;
                            break;
                        }
                    }
                    if (foundType)
                    {
                        fieldJson["type"] = "double";
                    }
                }

                if (!foundType)
                {
                    exampleArray = new JArray();
                    foundType = true;
                    foreach (string fieldValueString in allFieldValues)
                    {
                        if (float.TryParse(fieldValueString, out float fieldValue))
                        {
                            if (exampleArray.Count <= 10)
                            {
                                exampleArray.Add(fieldValue);
                            }
                        }
                        else
                        {
                            foundType = false;
                            break;
                        }
                    }
                    if (foundType)
                    {
                        fieldJson["type"] = "float";
                    }
                }

                if (!foundType)
                {
                    exampleArray = new JArray();
                    fieldJson["type"] = "string";
                    foreach (string fieldValue in allFieldValues)
                    {
                        if (exampleArray.Count <= 10)
                        {
                            exampleArray.Add(fieldValue);
                        }
                    }
                }

                fieldJson["examples"] = exampleArray;
                specJson[fieldName] = fieldJson;
            }

            return specJson;
        }



        public static JObject ProcessCSV(FileWrapper fileWrapper)
        {
            JObject specJson = FindFields(fileWrapper);
            FileInfo csvFile = fileWrapper.CreateInfo();

            JObject tableJson = new JObject();
            using (StreamReader streamReader = new StreamReader(csvFile.OpenRead(), Encoding.UTF8))
            {
                string[] fieldNames = streamReader.ReadLine().Split(",");
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    JObject JsonElement = new JObject();
                    string[] fieldValues = line.Split(",");

                    for (int i = 0; i < fieldValues.Length; i++)
                    {
                        string fieldName = fieldNames[i];
                        string fieldValue = fieldValues[i];

                        if ("".Equals(fieldValue))
                        {
                            continue;
                        }
                        string type = (string)((JObject)specJson[fieldName])["type"];
                        if ("int bool".Equals(type))
                        {
                            if ("0".Equals(fieldValue))
                            {
                                JsonElement[fieldName] = false;
                            }
                            else if ("1".Equals(fieldValue))
                            {
                                JsonElement[fieldName] = true;
                            }
                            else
                            {
                                throw new ArgumentException("Failed on " + csvFile + ". Bad key: " + type + ".");
                            }
                        }
                        else if ("string".Equals(type))
                        {
                            JsonElement[fieldName] = fieldValue;
                        }
                        else if ("float".Equals(type))
                        {
                            JsonElement[fieldName] = float.Parse(fieldValue);
                        }
                        else if ("double".Equals(type))
                        {
                            JsonElement[fieldName] = double.Parse(fieldValue);
                        }
                        else if ("int".Equals(type))
                        {
                            JsonElement[fieldName] = int.Parse(fieldValue);
                        }
                        else if ("bool".Equals(type))
                        {
                            JsonElement[fieldName] = bool.Parse(fieldValue);
                        }
                        else
                        {
                            throw new ArgumentException("Failed on " + csvFile + ". Bad key: " + type + ".");
                        }
                    }

                    if (fieldNames[0].Equals("id"))
                    {
                        if (JsonElement.ContainsKey(fieldValues[0]))
                        {
                            throw new ArgumentException("Failed on " + csvFile + ". Multiple values for " + fieldNames[0] + " are " + fieldValues[0]);
                        }
                        tableJson[fieldValues[0]] = JsonElement;
                    }
                    else
                    {
                        string primary_key = fieldValues[0];
                        string secondary_key = fieldValues[1];
                        JObject JsonDirectory = (JObject)tableJson[primary_key];

                        if (!tableJson.ContainsKey(primary_key))
                        {
                            tableJson[primary_key] = new JObject();
                            JsonDirectory = (JObject)tableJson[primary_key];
                        }
                        else if (JsonDirectory.ContainsKey(secondary_key))
                        {
                            throw new ArgumentException("Failed on " + csvFile + ". Multiple values for " + primary_key + ", " + secondary_key + " are " + fieldNames[0]);
                        }
                        JsonDirectory[secondary_key] = JsonElement;
                    }
                }
            }

            return tableJson;
        }

        public static JObject FindFields(FileWrapper fileWrapper)
        {
            Dictionary<string, HashSet<string>> fields = new Dictionary<string, HashSet<string>>();

            using (StreamReader streamReader = new StreamReader(new FileStream(fileWrapper.CreateInfo().ToString(), FileMode.Open, FileAccess.Read), Encoding.UTF8))
            {
                string[] fieldNames = streamReader.ReadLine().Split(",");
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    JObject JsonElement = new JObject();
                    string[] fieldValues = line.Split(",");
                    for (int i = 0; i < fieldValues.Length; i++)
                    {
                        string fieldName = fieldNames[i];
                        string fieldValue = fieldValues[i];

                        if ("".Equals(fieldValue))
                        {
                            continue;
                        }

                        if (!fields.ContainsKey(fieldName))
                        {
                            fields.Add(fieldName, new HashSet<string>());
                        }

                        fields[fieldName].Add(fieldValue);
                    }
                }
            }

            return FindFields(fields);
        }

        public static void WriteJson(string file, JToken token)
        {
            using (System.IO.StreamWriter fileWriter = new System.IO.StreamWriter(file))
            {
                fileWriter.WriteLine(token);
            }
        }
    }
}

