using CourseBack.Models;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using static Microsoft.ML.Transforms.Image.ImageResizingEstimator;

namespace CourseBack.Services
{
    public class YoloNetwork
    {

        static readonly string[] classesNames = new string[] { "Кровать", "Тумбочка", "Стеллаж", "Стул", "Диван", "Табурет", "Стол", "Шкаф" };

        public static IReadOnlyList<YoloRecognitionResult> MakeYoloPrediction(string imageUrl)
        {
            /*
            using (var client = new WebClient())
            {
                client.DownloadFile("https://neuralphotosblob.blob.core.windows.net/model/model_best.onnx", "model_best.onnx");
            }
            */

            MLContext mlContext = new MLContext();

                var pipeline = mlContext.Transforms.ResizeImages(inputColumnName: "bitmap", outputColumnName: "input_1:0", imageWidth: 416, imageHeight: 416, resizing: ResizingKind.IsoPad)
                   .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input_1:0", scaleImage: 1f / 255f, interleavePixelColors: true))
                   .Append(mlContext.Transforms.ApplyOnnxModel(
                       shapeDictionary: new Dictionary<string, int[]>()
                       {
                        { "input_1:0", new[] { 1, 416, 416, 3 } },
                        { "Identity:0", new[] { 1, 52, 52, 3, 85 } },
                        { "Identity_1:0", new[] { 1, 26, 26, 3, 85 } },
                        { "Identity_2:0", new[] { 1, 13, 13, 3, 85 } },
                       },
                       inputColumnNames: new[]
                       {
                        "input_1:0"
                       },
                       outputColumnNames: new[]
                       {
                        "tf_op_layer_concat_10",
                        "tf_op_layer_concat_11",
                        "tf_op_layer_concat_12"

                       },
                       modelFile: "model_best.onnx"));

                var model = pipeline.Fit(mlContext.Data.LoadFromEnumerable(new List<YoloImageData>()));

                var predictionEngine = mlContext.Model.CreatePredictionEngine<YoloImageData, YoloProcessor>(model);

                Bitmap bitmap = DownloadImageByUrl(imageUrl);

                var predict = predictionEngine.Predict(new YoloImageData() { Image = bitmap });
                var results = predict.ProcessResults(classesNames, 0.3f, 0.7f);

                return results;
        }

        private static Bitmap DownloadImageByUrl(string imageUrl)
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (Stream stream = client.OpenRead(imageUrl))
                    {
                        Bitmap bitmap = new Bitmap(stream);

                        if (bitmap != null)
                        {
                            bitmap.Save("currentyolo.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
                            return bitmap;
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
