using System;
using System.IO;
using Microsoft.ML.OnnxRuntime;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace CEPAggregator.Classes
{
	public class OnnxModelWrapper
	{
		public const int EOS = 1;
		public const int BOS = 2;
		public const int UNK = 3;
		private static readonly InferenceSession session;
		private const string pattern = @"[^\w\s+]";
		private static Data data;

		private class Data
        {
            public Dictionary<string, int> dict { get; set; }
            public List<string> stopwords { get; set; }
        }

		private static void ReadDict()
        {
			string jsonString = File.ReadAllText("wwwroot/res/dict.json");
			data = JsonConvert.DeserializeObject<Data>(jsonString);
        }

		static OnnxModelWrapper()
		{
			ReadDict();
			session = new InferenceSession("wwwroot/res/review_classifier.onnx");
		}

		private static List<string> Tokenize(string input)
        {
			var tokenized = Regex.Replace(input.ToLower(), pattern, " ").Split(" ");
			List<string> output = new List<string>();
			for (int i = 0; i < tokenized.Length; ++i)
            {
				if (tokenized[i].Length > 0 && !data.stopwords.Contains(tokenized[i]))
				{
					output.Add(tokenized[i]);
				}
			}
			return output;
        }

		private static Int64[] TextToMatrix(string input)
        {
			var tokens = Tokenize(input);
			Int64[] result = new Int64[tokens.Count + 2];
			result[0] = BOS;
			result[tokens.Count + 1] = EOS;
			for (var i = 0; i < tokens.Count; ++i)
            {
				if (data.dict.ContainsKey(tokens[i]))
                {
					result[i + 1] = data.dict[tokens[i]];
                }
				else
                {
					result[i + 1] = UNK;
                }
            }
			return result;
        }

		public static int Predict(string comment)
		{
			var matrix = TextToMatrix(comment);
			var inputTensor = new DenseTensor<Int64>(matrix, new int[] { 1, matrix.Length });
			var input = new List<NamedOnnxValue> { 
				NamedOnnxValue.CreateFromTensor("input", inputTensor) 
			};
			var output = session.Run(input);
			DenseTensor<float> outputTensor = null;
			foreach (var item in output)
            {
				outputTensor = (DenseTensor<float>)item.Value;
				break;
            }
			int maxIndex = 0;
			for (int i = 1; i < outputTensor.Length; ++i)
            {
				if (outputTensor.GetValue(maxIndex) < outputTensor.GetValue(i))
                {
					maxIndex = i;
                }
            }
			return maxIndex + 1;
		}
	}
}
